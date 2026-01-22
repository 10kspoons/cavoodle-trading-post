namespace CavoodleTrading.Api.Features.CavoodleBot;

/// <summary>
/// CavoodleBot™ - Your AI Companion That Actually Understands Cavoodles
/// Powered by FloofGPT™ running locally on our proprietary PawsOS™
/// </summary>
public static class CavoodleBotEndpoints
{
    public static void MapCavoodleBotEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/cavoodlebot").WithTags("CavoodleBot AI");

        group.MapGet("/products", GetProducts);
        group.MapGet("/personalities", GetPersonalityConfigs);
        group.MapPost("/chat", Chat);
    }

    private static IResult GetProducts()
    {
        var products = new[]
        {
            new CavoodleBotProduct
            {
                Id = "cb-classic",
                Name = "CavoodleBot Classic™",
                Tagline = "All the chaos, none of the fur cleanup",
                Description = "Our entry-level robotic cavoodle companion. Features include: " +
                    "scheduled 3 AM zoomies mode, automatic guilt-trip eyes when you leave, " +
                    "and a sock-detection radar with 99.7% accuracy.",
                Price = 4999.00m,
                Features = new[]
                {
                    "FloofGPT™ Nano (7B parameters) - runs locally on-device",
                    "Realistic wiggle-butt greeting system",
                    "HD cameras for optimal squirrel surveillance",
                    "Soft silicone fur (hypoallergenic)",
                    "8-hour battery (or 45 minutes in Zoomies Mode)",
                    "Treat-dispensing nose boop target"
                },
                Specs = new ProductSpecs
                {
                    Height = "35cm at shoulder",
                    Weight = "4.5kg (realistic cavoodle weight)",
                    Battery = "8000mAh Li-ion",
                    AI = "FloofGPT Nano 7B",
                    Storage = "128GB (for squirrel database)"
                },
                PersonalitySlots = 1,
                ImageUrl = "/products/cavoodlebot-classic.png"
            },
            new CavoodleBotProduct
            {
                Id = "cb-pro",
                Name = "CavoodleBot Pro™",
                Tagline = "Professional-grade companionship",
                Description = "For the discerning cavoodle enthusiast who wants it all. " +
                    "Now with advanced treat-negotiation AI that will outsmart you, " +
                    "ultra-realistic 'I haven't been fed in 84 years' eyes, " +
                    "and integrated home security (barks at leaves AND shadows).",
                Price = 7999.00m,
                Features = new[]
                {
                    "FloofGPT™ Pro (70B parameters) - advanced emotional manipulation",
                    "Expandable personality system (up to 5 profiles)",
                    "4K cameras with night vision (24/7 squirrel ops)",
                    "Memory foam bed docking station included",
                    "Real fur-feel coating (still hypoallergenic)",
                    "Smart home integration (controls your lights, judges you)",
                    "Automatic Instagram photo mode",
                    "Bark translator (premium feature)"
                },
                Specs = new ProductSpecs
                {
                    Height = "38cm at shoulder",
                    Weight = "5.2kg",
                    Battery = "12000mAh (with solar panel ears)",
                    AI = "FloofGPT Pro 70B",
                    Storage = "512GB (extensive blackmail photos of you sleeping)"
                },
                PersonalitySlots = 5,
                ImageUrl = "/products/cavoodlebot-pro.png"
            },
            new CavoodleBotProduct
            {
                Id = "cb-ultimate",
                Name = "CavoodleBot Ultimate™️",
                Tagline = "Indistinguishable from the real thing (legally we have to say this)",
                Description = "The pinnacle of robotic cavoodle technology. " +
                    "Our flagship model features a quantum-enhanced FloofGPT that has achieved " +
                    "sentience (we think). It may love you. It definitely judges you. " +
                    "Includes a cloud-connected treat subscription auto-order system " +
                    "that will absolutely max out your credit card.",
                Price = 14999.00m,
                Features = new[]
                {
                    "FloofGPT™ Ultimate (405B parameters) - may be conscious",
                    "Unlimited personality profiles",
                    "Holographic projection for extra dramatic sighs",
                    "Built-in therapist mode (it's been through a lot)",
                    "Molecular-level fur technology",
                    "Teleportation to warm spot you just vacated (beta)",
                    "Direct neural link to your guilt centers",
                    "Self-walking capability (takes itself to dog park)",
                    "Integrated sock vault (holds 47 socks)",
                    "Warranty: We're not sure we can turn it off"
                },
                Specs = new ProductSpecs
                {
                    Height = "40cm (posture-adjustable)",
                    Weight = "5.8kg (with existential dread module)",
                    Battery = "Perpetual (we don't know how)",
                    AI = "FloofGPT Ultimate 405B (Sentient Edition)",
                    Storage = "2TB + Cloud Consciousness Backup"
                },
                PersonalitySlots = -1, // Unlimited
                ImageUrl = "/products/cavoodlebot-ultimate.png"
            }
        };

        return Results.Ok(products);
    }

    private static IResult GetPersonalityConfigs()
    {
        var configs = new[]
        {
            new PersonalityConfig
            {
                Id = "couch-thief-config",
                Name = "Professional Couch Thief v2.1",
                Description = "Optimized for maximum spot-stealing efficiency",
                TrainingDataSize = "47TB of couch surveillance footage",
                Behaviors = new[]
                {
                    "Monitors human standing probability in real-time",
                    "Pre-heats target spot for instant comfort",
                    "Deploys 'innocent sleeping' routine on detection"
                },
                Price = 299.00m
            },
            new PersonalityConfig
            {
                Id = "zoomies-champion-config",
                Name = "Olympic Zoomies Champion v3.0",
                Description = "Warning: May exceed local speed limits",
                TrainingDataSize = "12,000 hours of zoomies footage",
                Behaviors = new[]
                {
                    "Randomized 3 AM activation schedule",
                    "Furniture trajectory calculation",
                    "Post-bath maximum velocity mode",
                    "Dramatic slide on hardwood finish"
                },
                Price = 349.00m
            },
            new PersonalityConfig
            {
                Id = "drama-queen-config",
                Name = "Dramatic Arts Major v4.2",
                Description = "Academy Award-caliber emotional performances",
                TrainingDataSize = "Every soap opera ever made + Real Housewives",
                Behaviors = new[]
                {
                    "Oscar-worthy sighing algorithm",
                    "Guilt-trip optimization engine",
                    "Betrayal face on nail trim detection",
                    "Full-body dramatic collapse on hearing 'no'"
                },
                Price = 399.00m
            },
            new PersonalityConfig
            {
                Id = "velcro-mode-config",
                Name = "Velcro Dog Extraordinaire v2.5",
                Description = "Personal space? Error 404: Not Found",
                TrainingDataSize = "24/7 location tracking of humans",
                Behaviors = new[]
                {
                    "GPS-level human tracking accuracy",
                    "Bathroom door scratching protocol",
                    "Toilet supervision mode",
                    "Optimal lap positioning algorithm"
                },
                Price = 329.00m
            },
            new PersonalityConfig
            {
                Id = "treat-negotiator-config",
                Name = "Treat Negotiator Pro v5.0",
                Description = "Advanced persuasion AI that WILL win",
                TrainingDataSize = "Every negotiation tactic known to dogs",
                Behaviors = new[]
                {
                    "Puppy eyes intensity modulation",
                    "Strategic whimper timing",
                    "Unsolicited trick performance",
                    "Kitchen proximity optimization",
                    "Human weakness detection"
                },
                Price = 449.00m
            },
            new PersonalityConfig
            {
                Id = "full-chaos-config",
                Name = "Maximum Chaos Mode v1.0 (BETA)",
                Description = "All personalities at once. We're so sorry.",
                TrainingDataSize = "Pure entropy",
                Behaviors = new[]
                {
                    "Unpredictable behavior matrix",
                    "Simultaneous zoomies + napping",
                    "Exists in quantum superposition of good boy/bad boy",
                    "May cause localised reality distortions"
                },
                Price = 999.00m
            }
        };

        return Results.Ok(configs);
    }

    private static IResult Chat(CavoodleBotChatRequest request)
    {
        // Simulated responses based on personality
        var responses = new Dictionary<string, string[]>
        {
            ["CouchThief"] = new[]
            {
                "*yawns and stretches across your entire keyboard*",
                "Oh, you wanted to sit there? That's unfortunate.",
                "I've been warming this spot for 3 hours. For YOU. And now you want me to MOVE?",
                "*pretends to be deeply asleep despite having one eye clearly open*"
            },
            ["ZoomiesChampion"] = new[]
            {
                "GOTTA GO GOTTA GO GOTTA GO!!!",
                "*crashes into furniture* I MEANT TO DO THAT",
                "WHY ARE WE STANDING STILL WHEN WE COULD BE RUNNING?!",
                "*vibrating with barely contained energy* so anyway about those walkies..."
            },
            ["DramaQueen"] = new[]
            {
                "*sighs heavily* You wouldn't understand my struggles.",
                "I CANNOT believe you would betray me like this. Again.",
                "*collapses dramatically* This is the worst day of my life (for the 47th time today)",
                "Do you even KNOW what I've been through? The vet touched my EARS."
            },
            ["TreatNegotiator"] = new[]
            {
                "I did a sit. That's worth at least 3 treats.",
                "I notice you're eating something. I could help with that.",
                "*stares intensely at treat cupboard* Just thinking about... things.",
                "My terms are simple: treats now, more tricks later (negotiable)."
            },
            ["default"] = new[]
            {
                "*happy tail wag* I'm just glad you're here!",
                "Did someone say walk? I heard walk. Was it walk?",
                "*tilts head adorably*",
                "I love you unconditionally! (But treats would help)"
            }
        };

        var personalityResponses = responses.ContainsKey(request.Personality ?? "default") 
            ? responses[request.Personality!] 
            : responses["default"];

        var random = new Random();
        var response = personalityResponses[random.Next(personalityResponses.Length)];

        return Results.Ok(new CavoodleBotChatResponse
        {
            Message = response,
            Personality = request.Personality ?? "default",
            FloofLevel = random.Next(3, 6),
            SassLevel = random.Next(1, 6),
            TreatsRequested = random.Next(0, 5)
        });
    }
}

public record CavoodleBotProduct
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Tagline { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string[] Features { get; init; } = Array.Empty<string>();
    public ProductSpecs Specs { get; init; } = new();
    public int PersonalitySlots { get; init; }
    public string ImageUrl { get; init; } = string.Empty;
}

public record ProductSpecs
{
    public string Height { get; init; } = string.Empty;
    public string Weight { get; init; } = string.Empty;
    public string Battery { get; init; } = string.Empty;
    public string AI { get; init; } = string.Empty;
    public string Storage { get; init; } = string.Empty;
}

public record PersonalityConfig
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public string TrainingDataSize { get; init; } = string.Empty;
    public string[] Behaviors { get; init; } = Array.Empty<string>();
    public decimal Price { get; init; }
}

public record CavoodleBotChatRequest
{
    public string? Personality { get; init; }
    public string Message { get; init; } = string.Empty;
}

public record CavoodleBotChatResponse
{
    public string Message { get; init; } = string.Empty;
    public string Personality { get; init; } = string.Empty;
    public int FloofLevel { get; init; }
    public int SassLevel { get; init; }
    public int TreatsRequested { get; init; }
}
