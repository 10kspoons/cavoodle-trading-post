using Microsoft.AspNetCore.Identity;
using CavoodleTrading.Api.Domain.Entities;

namespace CavoodleTrading.Api.Infrastructure.Identity;

public class ApplicationUser : IdentityUser<Guid>
{
    public string DisplayName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public int CavoodleKarma { get; set; } = 0;
    public string BadgesJson { get; set; } = "[]";  // JSON array of badge strings
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }

    // Navigation properties
    public ICollection<CavoodleListing> Listings { get; set; } = new List<CavoodleListing>();
    public ICollection<Review> ReviewsGiven { get; set; } = new List<Review>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    public ICollection<WatchlistItem> Watchlist { get; set; } = new List<WatchlistItem>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}
