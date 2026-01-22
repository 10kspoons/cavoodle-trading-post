using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using CavoodleTrading.Api.Infrastructure.Identity;

namespace CavoodleTrading.Api.Features.Auth;

public static class AuthEndpoints
{
    public static void MapAuthEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/auth").WithTags("Authentication");

        group.MapPost("/register", Register);
        group.MapPost("/login", Login);
        group.MapPost("/refresh", RefreshToken).RequireAuthorization();
    }

    private static async Task<IResult> Register(
        RegisterRequest request,
        UserManager<ApplicationUser> userManager,
        IConfiguration config)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            DisplayName = request.DisplayName,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(user, request.Password);
        
        if (!result.Succeeded)
        {
            return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        var token = GenerateJwtToken(user, config);
        
        return Results.Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email!
        });
    }

    private static async Task<IResult> Login(
        LoginRequest request,
        UserManager<ApplicationUser> userManager,
        IConfiguration config)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Results.Unauthorized();
        }

        user.LastLoginAt = DateTime.UtcNow;
        await userManager.UpdateAsync(user);

        var token = GenerateJwtToken(user, config);

        return Results.Ok(new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            DisplayName = user.DisplayName,
            Email = user.Email!
        });
    }

    private static async Task<IResult> RefreshToken(
        ClaimsPrincipal principal,
        UserManager<ApplicationUser> userManager,
        IConfiguration config)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Results.Unauthorized();

        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return Results.Unauthorized();

        var token = GenerateJwtToken(user, config);

        return Results.Ok(new { Token = token });
    }

    private static string GenerateJwtToken(ApplicationUser user, IConfiguration config)
    {
        var secret = config["Jwt:Secret"]!;
        var issuer = config["Jwt:Issuer"] ?? "cavoodle-trading-post";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email!),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim("karma", user.CavoodleKarma.ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public record RegisterRequest(string Email, string Password, string DisplayName);
public record LoginRequest(string Email, string Password);
public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public Guid UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}
