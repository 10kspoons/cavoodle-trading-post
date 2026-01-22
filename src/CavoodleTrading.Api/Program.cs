using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using CavoodleTrading.Api.Infrastructure.Data;
using CavoodleTrading.Api.Infrastructure.Identity;
using CavoodleTrading.Api.Features.Auth;
using CavoodleTrading.Api.Features.Listings;
using CavoodleTrading.Api.Features.PersonalityQuiz;
using CavoodleTrading.Api.Features.Users;
using CavoodleTrading.Api.Features.Messaging;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// =============================================================================
// Services
// =============================================================================

// Database
builder.Services.AddDbContext<CavoodleDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQL")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 8;
    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<CavoodleDbContext>()
.AddDefaultTokenProviders();

// JWT Authentication
var jwtSecret = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret not configured");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "cavoodle-trading-post";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
    };

    // Support SignalR JWT via query string
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Redis Cache (optional - graceful fallback)
var redisConnection = builder.Configuration.GetConnectionString("Redis");
if (!string.IsNullOrEmpty(redisConnection))
{
    builder.Services.AddStackExchangeRedisCache(options =>
    {
        options.Configuration = redisConnection;
        options.InstanceName = "cavoodle:";
    });
}
else
{
    builder.Services.AddDistributedMemoryCache();
}

// SignalR for real-time messaging
builder.Services.AddSignalR();

// OpenAPI / Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "The Cavoodle Trading Post API",
        Version = "v1",
        Description = "🐕 A quirky marketplace for buying and selling cavoodles"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// CORS for Flutter clients
builder.Services.AddCors(options =>
{
    options.AddPolicy("CavoodlePolicy", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:Origins").Get<string[]>() 
            ?? new[] { "http://localhost:3000", "http://localhost:8080" };
        
        policy.WithOrigins(origins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

var app = builder.Build();

// =============================================================================
// Middleware
// =============================================================================

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cavoodle Trading Post API v1");
    c.RoutePrefix = "swagger";
});

app.UseCors("CavoodlePolicy");
app.UseAuthentication();
app.UseAuthorization();

// =============================================================================
// Endpoints
// =============================================================================

app.MapAuthEndpoints();
app.MapListingsEndpoints();
app.MapQuizEndpoints();
app.MapUsersEndpoints();
app.MapMessagingEndpoints();

// SignalR Hub
app.MapHub<MessagingHub>("/hubs/messaging");

// Health check
app.MapGet("/health", () => Results.Ok(new 
{ 
    Status = "Healthy", 
    Service = "CavoodleTrading.Api",
    Timestamp = DateTime.UtcNow
})).WithTags("Health");

// Apply migrations on startup (dev convenience)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<CavoodleDbContext>();
    db.Database.Migrate();
}

app.Run();
