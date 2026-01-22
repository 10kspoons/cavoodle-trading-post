using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CavoodleTrading.Api.Domain.Entities;
using CavoodleTrading.Api.Infrastructure.Identity;

namespace CavoodleTrading.Api.Infrastructure.Data;

public class CavoodleDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
{
    public CavoodleDbContext(DbContextOptions<CavoodleDbContext> options) : base(options)
    {
    }

    public DbSet<CavoodleListing> Listings => Set<CavoodleListing>();
    public DbSet<PersonalityProfile> PersonalityProfiles => Set<PersonalityProfile>();
    public DbSet<ListingPhoto> ListingPhotos => Set<ListingPhoto>();
    public DbSet<Conversation> Conversations => Set<Conversation>();
    public DbSet<ConversationParticipant> ConversationParticipants => Set<ConversationParticipant>();
    public DbSet<Message> Messages => Set<Message>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<WatchlistItem> WatchlistItems => Set<WatchlistItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // CavoodleListing
        builder.Entity<CavoodleListing>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Price).HasPrecision(10, 2);
            entity.Property(e => e.Suburb).HasMaxLength(100);
            entity.Property(e => e.State).HasMaxLength(50);
            
            entity.HasOne(e => e.Seller)
                .WithMany(u => u.Listings)
                .HasForeignKey(e => e.SellerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.PersonalityProfile)
                .WithOne(p => p.Listing)
                .HasForeignKey<PersonalityProfile>(p => p.ListingId);

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CreatedAt);
        });

        // PersonalityProfile
        builder.Entity<PersonalityProfile>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.ListingId).IsUnique();
        });

        // ListingPhoto
        builder.Entity<ListingPhoto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Url).HasMaxLength(500).IsRequired();
            
            entity.HasOne(e => e.Listing)
                .WithMany(l => l.Photos)
                .HasForeignKey(e => e.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Conversation
        builder.Entity<Conversation>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Listing)
                .WithMany()
                .HasForeignKey(e => e.ListingId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // ConversationParticipant (many-to-many join table)
        builder.Entity<ConversationParticipant>(entity =>
        {
            entity.HasKey(e => new { e.ConversationId, e.UserId });
            
            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Participants)
                .HasForeignKey(e => e.ConversationId);

            entity.HasOne(e => e.User)
                .WithMany()
                .HasForeignKey(e => e.UserId);
        });

        // Message
        builder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Content).IsRequired();
            
            entity.HasOne(e => e.Conversation)
                .WithMany(c => c.Messages)
                .HasForeignKey(e => e.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(e => e.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CreatedAt);
        });

        // Review
        builder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Rating).IsRequired();
            
            entity.HasOne(e => e.Reviewer)
                .WithMany(u => u.ReviewsGiven)
                .HasForeignKey(e => e.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Reviewee)
                .WithMany(u => u.ReviewsReceived)
                .HasForeignKey(e => e.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Listing)
                .WithMany()
                .HasForeignKey(e => e.ListingId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        // WatchlistItem
        builder.Entity<WatchlistItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.UserId, e.ListingId }).IsUnique();
            
            entity.HasOne(e => e.User)
                .WithMany(u => u.Watchlist)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Listing)
                .WithMany()
                .HasForeignKey(e => e.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
