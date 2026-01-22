using CavoodleTrading.Api.Domain.Enums;
using CavoodleTrading.Api.Infrastructure.Identity;

namespace CavoodleTrading.Api.Domain.Entities;

public class CavoodleListing
{
    public Guid Id { get; set; }
    public Guid SellerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int AgeMonths { get; set; }
    public CavoodleColor Color { get; set; }
    public CavoodleGender Gender { get; set; }
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public ListingStatus Status { get; set; } = ListingStatus.Draft;
    public string Suburb { get; set; } = string.Empty;
    public string State { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    public ApplicationUser Seller { get; set; } = null!;
    public PersonalityProfile? PersonalityProfile { get; set; }
    public ICollection<ListingPhoto> Photos { get; set; } = new List<ListingPhoto>();
}

public class PersonalityProfile
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public PersonalityType Type { get; set; }
    public int EnergyLevel { get; set; }    // 1-5
    public int FloofFactor { get; set; }    // 1-5 (fluffiness rating)
    public int SassRating { get; set; }     // 1-5 (attitude level)
    public string? CustomBio { get; set; }
    public string? QuizAnswersJson { get; set; }  // Store quiz answers as JSON

    // Navigation
    public CavoodleListing Listing { get; set; } = null!;
}

public class ListingPhoto
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public string Url { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsPrimary { get; set; }

    // Navigation
    public CavoodleListing Listing { get; set; } = null!;
}

public class Conversation
{
    public Guid Id { get; set; }
    public Guid ListingId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastMessageAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public CavoodleListing Listing { get; set; } = null!;
    public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    public ICollection<Message> Messages { get; set; } = new List<Message>();
}

public class ConversationParticipant
{
    public Guid ConversationId { get; set; }
    public Guid UserId { get; set; }

    // Navigation
    public Conversation Conversation { get; set; } = null!;
    public ApplicationUser User { get; set; } = null!;
}

public class Message
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }
    public Guid SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public Conversation Conversation { get; set; } = null!;
    public ApplicationUser Sender { get; set; } = null!;
}

public class Review
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid RevieweeId { get; set; }
    public Guid? ListingId { get; set; }
    public int Rating { get; set; }  // 1-5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser Reviewer { get; set; } = null!;
    public ApplicationUser Reviewee { get; set; } = null!;
    public CavoodleListing? Listing { get; set; }
}

public class WatchlistItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ListingId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public ApplicationUser User { get; set; } = null!;
    public CavoodleListing Listing { get; set; } = null!;
}
