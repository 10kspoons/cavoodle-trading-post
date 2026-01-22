using CavoodleTrading.Api.Domain.Entities;
using CavoodleTrading.Api.Domain.Enums;
using CavoodleTrading.Api.Infrastructure.Data;
using CavoodleTrading.Api.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CavoodleTrading.Api.Features.Seed;

public static class SeedEndpoints
{
    public static void MapSeedEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/seed").WithTags("Seed Data");

        group.MapPost("/", SeedDatabase)
            .WithName("SeedDatabase")
            .WithDescription("Seeds the database with sample cavoodle listings");
    }

    private static async Task<IResult> SeedDatabase(
        CavoodleDbContext db,
        UserManager<ApplicationUser> userManager)
    {
        // Check if already seeded
        if (await db.Listings.AnyAsync())
        {
            return Results.Ok(new { message = "Database already seeded", count = await db.Listings.CountAsync() });
        }

        // Create sample users
        var sellers = new List<ApplicationUser>();
        var sellerData = new[]
        {
            ("fluffylover@example.com", "FluffyDogLover", 850),
            ("pawsome@example.com", "PawsomePups", 1200),
            ("cavoodleking@example.com", "TheCavoodleKing", 2100),
            ("doglady@example.com", "SydneyDogMum", 650),
            ("puppypalace@example.com", "PuppyPalaceAU", 1800),
        };

        foreach (var (email, displayName, karma) in sellerData)
        {
            var user = new ApplicationUser
            {
                Email = email,
                UserName = email,
                DisplayName = displayName,
                CavoodleKarma = karma,
                BadgesJson = karma > 1000 ? "[\"Top Seller\",\"Verified\"]" : "[\"Verified\"]",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(user, "SamplePass123!");
            sellers.Add(user);
        }

        // Sample listings data
        var listings = new List<CavoodleListing>
        {
            new()
            {
                Name = "Biscuit",
                Description = "Meet Biscuit! This adorable cream-colored bundle of joy is the perfect companion for any family. Biscuit loves cuddles, gentle walks, and has mastered the art of looking irresistibly cute. He's fully vaccinated, microchipped, and comes with his favorite squeaky toy. This little guy has never met a stranger - everyone is his best friend!",
                Price = 3500,
                AgeMonths = 3,
                Color = CavoodleColor.Cream,
                Gender = CavoodleGender.Male,
                Suburb = "Bondi",
                State = "NSW",
                Status = ListingStatus.Active,
                SellerId = sellers[0].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                Photos = new List<ListingPhoto>
                {
                    new() { Url = "/sample/cream_puppy.png", IsPrimary = true, DisplayOrder = 0 }
                },
                PersonalityProfile = new PersonalityProfile
                {
                    Type = PersonalityType.CouchThief,
                    EnergyLevel = 4,
                    FloofFactor = 5,
                    SassRating = 3,
                    CustomBio = "Expert snuggler, professional treat negotiator"
                }
            },
            new()
            {
                Name = "Caramel",
                Description = "Caramel is a gorgeous apricot adult cavoodle looking for her forever home. She's a calm, loving soul who enjoys lazy Sunday mornings and Netflix marathons. House-trained, great with kids, and gets along wonderfully with other pets. Her previous owner had to relocate overseas. All vet records available.",
                Price = 2800,
                AgeMonths = 24,
                Color = CavoodleColor.Apricot,
                Gender = CavoodleGender.Female,
                Suburb = "Melbourne CBD",
                State = "VIC",
                Status = ListingStatus.Active,
                SellerId = sellers[1].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-3),
                Photos = new List<ListingPhoto>
                {
                    new() { Url = "/sample/apricot_adult.png", IsPrimary = true, DisplayOrder = 0 }
                },
                PersonalityProfile = new PersonalityProfile
                {
                    Type = PersonalityType.ZoomiesChampion,
                    EnergyLevel = 3,
                    FloofFactor = 4,
                    SassRating = 2,
                    CustomBio = "Queen of the couch, expert belly rub receiver"
                }
            },
            new()
            {
                Name = "Cocoa",
                Description = "ENERGY ALERT! 🚀 Cocoa is a chocolate-colored dynamo who lives for playtime! If you're active and love outdoor adventures, Cocoa is your perfect match. He can fetch for hours, loves swimming, and has been to puppy school where he graduated top of his class (mostly). Needs a home with a backyard!",
                Price = 4200,
                AgeMonths = 8,
                Color = CavoodleColor.Chocolate,
                Gender = CavoodleGender.Male,
                Suburb = "Gold Coast",
                State = "QLD",
                Status = ListingStatus.Active,
                SellerId = sellers[2].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Photos = new List<ListingPhoto>
                {
                    new() { Url = "/sample/chocolate_playful.png", IsPrimary = true, DisplayOrder = 0 }
                },
                PersonalityProfile = new PersonalityProfile
                {
                    Type = PersonalityType.TreatNegotiator,
                    EnergyLevel = 5,
                    FloofFactor = 4,
                    SassRating = 4,
                    CustomBio = "Born to run, forced to wait for walkies"
                }
            },
            new()
            {
                Name = "Shadow",
                Description = "Shadow is a stunning black cavoodle with an aristocratic air. Don't let his elegant looks fool you - he's a total goofball at home! Shadow is perfect for apartment living, loves learning tricks, and has impeccable manners. He's great for first-time dog owners and is very low-shedding.",
                Price = 3800,
                AgeMonths = 12,
                Color = CavoodleColor.Black,
                Gender = CavoodleGender.Male,
                Suburb = "Perth",
                State = "WA",
                Status = ListingStatus.Active,
                SellerId = sellers[3].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-7),
                Photos = new List<ListingPhoto>
                {
                    new() { Url = "/sample/black_elegant.png", IsPrimary = true, DisplayOrder = 0 }
                },
                PersonalityProfile = new PersonalityProfile
                {
                    Type = PersonalityType.DramaQueen,
                    EnergyLevel = 3,
                    FloofFactor = 5,
                    SassRating = 5,
                    CustomBio = "Sophisticated on the outside, silly on the inside"
                }
            },
            new()
            {
                Name = "Maple",
                Description = "Red-hot cuteness alert! 🔥 Maple is an absolute stunner with her gorgeous auburn coat. She's adventurous, loves car rides, and dreams of becoming an Instagram influencer (aren't we all?). Maple comes from champion bloodlines and has been health-tested for all common conditions.",
                Price = 4500,
                AgeMonths = 6,
                Color = CavoodleColor.Red,
                Gender = CavoodleGender.Female,
                Suburb = "Adelaide",
                State = "SA",
                Status = ListingStatus.Active,
                SellerId = sellers[4].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                Photos = new List<ListingPhoto>
                {
                    new() { Url = "/sample/red_outdoor.png", IsPrimary = true, DisplayOrder = 0 }
                },
                PersonalityProfile = new PersonalityProfile
                {
                    Type = PersonalityType.VelcroVelvet,
                    EnergyLevel = 4,
                    FloofFactor = 5,
                    SassRating = 3,
                    CustomBio = "Adventure buddy seeking human sidekick"
                }
            },
            new()
            {
                Name = "Oreo",
                Description = "Oreo is a rare phantom-colored cavoodle with the most beautiful black and tan markings. She's a professional napper and part-time cuddle therapist. Oreo is great with seniors and would make an excellent emotional support companion. She's calm, affectionate, and low-maintenance.",
                Price = 3200,
                AgeMonths = 36,
                Color = CavoodleColor.Phantom,
                Gender = CavoodleGender.Female,
                Suburb = "Hobart",
                State = "TAS",
                Status = ListingStatus.Active,
                SellerId = sellers[0].Id,
                CreatedAt = DateTime.UtcNow.AddDays(-10),
                Photos = new List<ListingPhoto>
                {
                    new() { Url = "/sample/phantom_cozy.png", IsPrimary = true, DisplayOrder = 0 }
                },
                PersonalityProfile = new PersonalityProfile
                {
                    Type = PersonalityType.SockCollector,
                    EnergyLevel = 2,
                    FloofFactor = 4,
                    SassRating = 2,
                    CustomBio = "Certified therapy floof, PhD in napping"
                }
            }
        };

        db.Listings.AddRange(listings);
        await db.SaveChangesAsync();

        return Results.Ok(new 
        { 
            message = "Database seeded successfully! 🐕",
            listings = listings.Count,
            users = sellers.Count
        });
    }
}
