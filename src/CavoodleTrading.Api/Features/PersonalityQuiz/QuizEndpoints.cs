using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using CavoodleTrading.Api.Domain.Entities;
using CavoodleTrading.Api.Domain.Enums;
using CavoodleTrading.Api.Infrastructure.Data;

namespace CavoodleTrading.Api.Features.PersonalityQuiz;

public static class QuizEndpoints
{
    public static void MapQuizEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/quiz").WithTags("Personality Quiz");

        group.MapGet("/questions", GetQuestions);
        group.MapPost("/calculate", CalculatePersonality);
        group.MapPost("/{listingId:guid}/submit", SubmitQuiz).RequireAuthorization();
    }

    private static IResult GetQuestions()
    {
        return Results.Ok(QuizQuestions.Questions);
    }

    private static IResult CalculatePersonality(QuizAnswersRequest request)
    {
        var result = PersonalityCalculator.Calculate(request.Answers);
        return Results.Ok(result);
    }

    private static async Task<IResult> SubmitQuiz(
        Guid listingId,
        QuizAnswersRequest request,
        ClaimsPrincipal principal,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var listing = await db.Listings
            .Include(l => l.PersonalityProfile)
            .FirstOrDefaultAsync(l => l.Id == listingId && l.SellerId == userId);

        if (listing == null)
        {
            return Results.NotFound();
        }

        var result = PersonalityCalculator.Calculate(request.Answers);
        
        if (listing.PersonalityProfile == null)
        {
            listing.PersonalityProfile = new PersonalityProfile
            {
                Id = Guid.NewGuid(),
                ListingId = listingId
            };
            db.PersonalityProfiles.Add(listing.PersonalityProfile);
        }

        listing.PersonalityProfile.Type = Enum.Parse<PersonalityType>(result.PersonalityType);
        listing.PersonalityProfile.EnergyLevel = result.EnergyLevel;
        listing.PersonalityProfile.FloofFactor = result.FloofFactor;
        listing.PersonalityProfile.SassRating = result.SassRating;
        listing.PersonalityProfile.CustomBio = result.CustomBio;
        listing.PersonalityProfile.QuizAnswersJson = JsonSerializer.Serialize(request.Answers);

        await db.SaveChangesAsync();

        return Results.Ok(result);
    }
}

public static class QuizQuestions
{
    public static readonly List<QuizQuestion> Questions = new()
    {
        new QuizQuestion
        {
            Id = 1,
            Question = "When the doorbell rings, your cavoodle...",
            Options = new List<QuizOption>
            {
                new("A", "Loses their absolute mind (5 mins of barking minimum)", new Dictionary<string, int> { ["bark"] = 5, ["energy"] = 4 }),
                new("B", "Does a single boof then returns to napping", new Dictionary<string, int> { ["chill"] = 4, ["energy"] = 1 }),
                new("C", "Sprints to the door hoping for treats", new Dictionary<string, int> { ["treat"] = 5, ["energy"] = 3 }),
                new("D", "Hides behind the couch (stranger danger!)", new Dictionary<string, int> { ["drama"] = 3, ["sass"] = 2 })
            }
        },
        new QuizQuestion
        {
            Id = 2,
            Question = "Your cavoodle's relationship with socks is best described as...",
            Options = new List<QuizOption>
            {
                new("A", "Collector - has a secret stash somewhere", new Dictionary<string, int> { ["sock"] = 5 }),
                new("B", "Destroyer - no sock survives", new Dictionary<string, int> { ["energy"] = 4, ["sass"] = 3 }),
                new("C", "Indifferent - socks are boring", new Dictionary<string, int> { ["chill"] = 3 }),
                new("D", "Protector - guards them from the laundry basket", new Dictionary<string, int> { ["drama"] = 4 })
            }
        },
        new QuizQuestion
        {
            Id = 3,
            Question = "At the dog park, your cavoodle is...",
            Options = new List<QuizOption>
            {
                new("A", "The social butterfly (must greet EVERYONE)", new Dictionary<string, int> { ["velcro"] = 4, ["energy"] = 4 }),
                new("B", "The zoomies champion (blur of floof)", new Dictionary<string, int> { ["zoomies"] = 5, ["energy"] = 5 }),
                new("C", "The benchwarmer (prefers human company)", new Dictionary<string, int> { ["velcro"] = 5, ["chill"] = 3 }),
                new("D", "The referee (breaks up all the fun)", new Dictionary<string, int> { ["bark"] = 3, ["sass"] = 4 })
            }
        },
        new QuizQuestion
        {
            Id = 4,
            Question = "When it comes to the couch, your cavoodle believes...",
            Options = new List<QuizOption>
            {
                new("A", "It's THEIR couch. You're just visiting.", new Dictionary<string, int> { ["couch"] = 5, ["sass"] = 5 }),
                new("B", "Sharing is caring (but they get the best spot)", new Dictionary<string, int> { ["couch"] = 3, ["velcro"] = 3 }),
                new("C", "The floor is perfectly acceptable", new Dictionary<string, int> { ["chill"] = 4 }),
                new("D", "Why sit when you can zoom?", new Dictionary<string, int> { ["zoomies"] = 4, ["energy"] = 4 })
            }
        },
        new QuizQuestion
        {
            Id = 5,
            Question = "During a thunderstorm, your cavoodle...",
            Options = new List<QuizOption>
            {
                new("A", "Dramatically trembles like they're in a soap opera", new Dictionary<string, int> { ["drama"] = 5 }),
                new("B", "Sleeps through it like nothing's happening", new Dictionary<string, int> { ["chill"] = 5 }),
                new("C", "Barks at the thunder to show it who's boss", new Dictionary<string, int> { ["bark"] = 4, ["sass"] = 3 }),
                new("D", "Becomes a permanent lap attachment", new Dictionary<string, int> { ["velcro"] = 5 })
            }
        },
        new QuizQuestion
        {
            Id = 6,
            Question = "When you try to leave the house, your cavoodle...",
            Options = new List<QuizOption>
            {
                new("A", "Gives you guilt-trip eyes that could win an Oscar", new Dictionary<string, int> { ["drama"] = 4, ["velcro"] = 4 }),
                new("B", "Doesn't notice because they're napping", new Dictionary<string, int> { ["chill"] = 5 }),
                new("C", "Tries to fit into your bag", new Dictionary<string, int> { ["velcro"] = 5, ["sass"] = 2 }),
                new("D", "Barks instructions at you through the window", new Dictionary<string, int> { ["bark"] = 4, ["sass"] = 4 })
            }
        },
        new QuizQuestion
        {
            Id = 7,
            Question = "Your cavoodle's approach to treat negotiations is...",
            Options = new List<QuizOption>
            {
                new("A", "Professional-grade puppy eyes and tactical whining", new Dictionary<string, int> { ["treat"] = 5, ["sass"] = 3 }),
                new("B", "Performs their entire trick repertoire unsolicited", new Dictionary<string, int> { ["treat"] = 4, ["energy"] = 3 }),
                new("C", "Sits politely and waits (mostly)", new Dictionary<string, int> { ["chill"] = 3, ["treat"] = 2 }),
                new("D", "Attempts to open the treat cupboard themselves", new Dictionary<string, int> { ["treat"] = 5, ["sass"] = 5, ["garden"] = 2 })
            }
        }
    };
}

public static class PersonalityCalculator
{
    public static PersonalityResult Calculate(Dictionary<int, string> answers)
    {
        var scores = new Dictionary<string, int>
        {
            ["couch"] = 0, ["zoomies"] = 0, ["treat"] = 0, ["sock"] = 0,
            ["bark"] = 0, ["velcro"] = 0, ["garden"] = 0, ["drama"] = 0,
            ["energy"] = 0, ["sass"] = 0, ["chill"] = 0
        };

        foreach (var (questionId, answerId) in answers)
        {
            var question = QuizQuestions.Questions.FirstOrDefault(q => q.Id == questionId);
            var option = question?.Options.FirstOrDefault(o => o.Id == answerId);
            
            if (option != null)
            {
                foreach (var (trait, value) in option.Scores)
                {
                    if (scores.ContainsKey(trait))
                        scores[trait] += value;
                }
            }
        }

        // Determine personality type based on highest scores
        var personalityType = DeterminePersonalityType(scores);
        
        return new PersonalityResult
        {
            PersonalityType = personalityType.ToString(),
            PersonalityDisplayName = GetDisplayName(personalityType),
            EnergyLevel = Math.Clamp(scores["energy"] / 4, 1, 5),
            FloofFactor = new Random().Next(3, 6), // Random floof (it's about the coat!)
            SassRating = Math.Clamp(scores["sass"] / 3, 1, 5),
            CustomBio = GenerateBio(personalityType, scores)
        };
    }

    private static PersonalityType DeterminePersonalityType(Dictionary<string, int> scores)
    {
        var typeScores = new Dictionary<PersonalityType, int>
        {
            [PersonalityType.CouchThief] = scores["couch"] + scores["sass"],
            [PersonalityType.ZoomiesChampion] = scores["zoomies"] + scores["energy"],
            [PersonalityType.TreatNegotiator] = scores["treat"] + scores["sass"],
            [PersonalityType.SockCollector] = scores["sock"] + scores["garden"],
            [PersonalityType.BarkingBarrister] = scores["bark"] + scores["sass"],
            [PersonalityType.VelcroVelvet] = scores["velcro"] + scores["chill"],
            [PersonalityType.GardenDestroyer] = scores["garden"] + scores["energy"],
            [PersonalityType.DramaQueen] = scores["drama"] + scores["sass"]
        };

        return typeScores.OrderByDescending(x => x.Value).First().Key;
    }

    private static string GetDisplayName(PersonalityType type) => type switch
    {
        PersonalityType.CouchThief => "Professional Couch Thief",
        PersonalityType.ZoomiesChampion => "Olympic Zoomies Champion",
        PersonalityType.TreatNegotiator => "Professional Treat Negotiator",
        PersonalityType.SockCollector => "Certified Sock Collector",
        PersonalityType.BarkingBarrister => "Barking Barrister",
        PersonalityType.VelcroVelvet => "Velcro Dog Extraordinaire",
        PersonalityType.GardenDestroyer => "Landscape Architect",
        PersonalityType.DramaQueen => "Dramatic Arts Major",
        _ => type.ToString()
    };

    private static string GenerateBio(PersonalityType type, Dictionary<string, int> scores)
    {
        return type switch
        {
            PersonalityType.CouchThief => "Will steal your spot the moment you stand up. Has perfected the art of looking innocent.",
            PersonalityType.ZoomiesChampion => "Powered by an unknown energy source. May spontaneously achieve lightspeed.",
            PersonalityType.TreatNegotiator => "Has never met a treat they didn't deserve. Expert in puppy-eye diplomacy.",
            PersonalityType.SockCollector => "Curator of a private sock museum. Location: classified.",
            PersonalityType.BarkingBarrister => "Objects to everything. Loudly. The wind? Objectionable. Leaves? Also objectionable.",
            PersonalityType.VelcroVelvet => "Personal space is a myth. Your lap is their office.",
            PersonalityType.GardenDestroyer => "Has redesigned your backyard. You're welcome.",
            PersonalityType.DramaQueen => "Oscar-worthy performances daily. Specializes in betrayed looks.",
            _ => "A unique and wonderful cavoodle!"
        };
    }
}

public record QuizQuestion
{
    public int Id { get; init; }
    public string Question { get; init; } = string.Empty;
    public List<QuizOption> Options { get; init; } = new();
}

public record QuizOption(string Id, string Text, Dictionary<string, int> Scores);

public record QuizAnswersRequest(Dictionary<int, string> Answers);

public record PersonalityResult
{
    public string PersonalityType { get; init; } = string.Empty;
    public string PersonalityDisplayName { get; init; } = string.Empty;
    public int EnergyLevel { get; init; }
    public int FloofFactor { get; init; }
    public int SassRating { get; init; }
    public string? CustomBio { get; init; }
}
