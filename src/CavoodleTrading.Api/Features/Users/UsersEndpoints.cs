using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CavoodleTrading.Api.Infrastructure.Data;
using CavoodleTrading.Api.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;

namespace CavoodleTrading.Api.Features.Users;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/me", GetCurrentUser).RequireAuthorization();
        group.MapGet("/{id:guid}", GetUser);
        group.MapPut("/me", UpdateProfile).RequireAuthorization();
    }

    private static async Task<IResult> GetCurrentUser(
        ClaimsPrincipal principal,
        UserManager<ApplicationUser> userManager,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var user = await userManager.FindByIdAsync(userId.ToString());
        
        if (user == null)
        {
            return Results.NotFound();
        }

        var listingCount = await db.Listings.CountAsync(l => l.SellerId == userId);
        var reviewCount = await db.Reviews.CountAsync(r => r.RevieweeId == userId);

        return Results.Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email!,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            CavoodleKarma = user.CavoodleKarma,
            Badges = System.Text.Json.JsonSerializer.Deserialize<List<string>>(user.BadgesJson) ?? new(),
            CreatedAt = user.CreatedAt,
            ListingCount = listingCount,
            ReviewCount = reviewCount
        });
    }

    private static async Task<IResult> GetUser(
        Guid id,
        UserManager<ApplicationUser> userManager,
        CavoodleDbContext db)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        
        if (user == null)
        {
            return Results.NotFound();
        }

        var listingCount = await db.Listings.CountAsync(l => l.SellerId == id);
        var reviews = await db.Reviews
            .Where(r => r.RevieweeId == id)
            .OrderByDescending(r => r.CreatedAt)
            .Take(10)
            .Select(r => new ReviewDto
            {
                Rating = r.Rating,
                Comment = r.Comment,
                ReviewerName = r.Reviewer.DisplayName,
                CreatedAt = r.CreatedAt
            })
            .ToListAsync();

        return Results.Ok(new PublicUserProfileDto
        {
            Id = user.Id,
            DisplayName = user.DisplayName,
            AvatarUrl = user.AvatarUrl,
            CavoodleKarma = user.CavoodleKarma,
            Badges = System.Text.Json.JsonSerializer.Deserialize<List<string>>(user.BadgesJson) ?? new(),
            MemberSince = user.CreatedAt,
            ListingCount = listingCount,
            RecentReviews = reviews
        });
    }

    private static async Task<IResult> UpdateProfile(
        UpdateProfileRequest request,
        ClaimsPrincipal principal,
        UserManager<ApplicationUser> userManager)
    {
        var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = await userManager.FindByIdAsync(userId);
        
        if (user == null)
        {
            return Results.NotFound();
        }

        if (!string.IsNullOrEmpty(request.DisplayName))
        {
            user.DisplayName = request.DisplayName;
        }

        if (request.AvatarUrl != null)
        {
            user.AvatarUrl = request.AvatarUrl;
        }

        var result = await userManager.UpdateAsync(user);
        
        if (!result.Succeeded)
        {
            return Results.BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
        }

        return Results.Ok(new { Message = "Profile updated successfully" });
    }
}

public record UserProfileDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public int CavoodleKarma { get; init; }
    public List<string> Badges { get; init; } = new();
    public DateTime CreatedAt { get; init; }
    public int ListingCount { get; init; }
    public int ReviewCount { get; init; }
}

public record PublicUserProfileDto
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public int CavoodleKarma { get; init; }
    public List<string> Badges { get; init; } = new();
    public DateTime MemberSince { get; init; }
    public int ListingCount { get; init; }
    public List<ReviewDto> RecentReviews { get; init; } = new();
}

public record ReviewDto
{
    public int Rating { get; init; }
    public string? Comment { get; init; }
    public string ReviewerName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
}

public record UpdateProfileRequest(string? DisplayName = null, string? AvatarUrl = null);
