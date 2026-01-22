using CavoodleTrading.Api.Domain.Enums;

namespace CavoodleTrading.Api.Features.PersonalityQuiz;

/// <summary>
/// The definitive guide to Cavoodle personality types.
/// Scientifically validated by watching 10,000+ cavoodle TikToks.
/// </summary>
public static class PersonalityDescriptions
{
    public static PersonalityInfo GetInfo(PersonalityType type) => type switch
    {
        PersonalityType.CouchThief => new(
            Title: "🛋️ Professional Couch Thief",
            Tagline: "Your spot? My spot now.",
            Description: "The moment you get up - even for a SECOND - this floofy opportunist has claimed your warm spot. They've perfected the art of looking deeply asleep and impossible to move. Scientists believe they can sense your intent to stand approximately 0.3 seconds before you do.",
            Strengths: ["Expert at thermoregulation (via your body heat)", "Olympic-level sprawling", "Academy Award-worthy fake sleeping"],
            Weaknesses: ["Absolutely zero respect for personal furniture boundaries", "Will hold grudges if moved"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be George Costanza",
            RarityLevel: "Very Common",
            Emoji: "🛋️",
            TraitScores: new(Energy: 2, Floof: 4, Sass: 5, Cuddle: 5, Chaos: 3)
        ),
        
        PersonalityType.CushionEngineer => new(
            Title: "🏗️ Certified Cushion Engineer",
            Tagline: "This nest isn't going to build itself.",
            Description: "Armed with an architecture degree from Floof University, this cavoodle spends hours rearranging every cushion, blanket, and pillow into the PERFECT nest. Don't you dare disturb their masterpiece. They know it was moved 2mm to the left.",
            Strengths: ["Interior design visionary", "Attention to comfort detail", "Never cold"],
            Weaknesses: ["Your bed is now exclusively theirs", "Throws tantrums if someone sits on their creation"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be Martha Stewart",
            RarityLevel: "Common",
            Emoji: "🏗️",
            TraitScores: new(Energy: 2, Floof: 5, Sass: 4, Cuddle: 4, Chaos: 2)
        ),
        
        PersonalityType.ZoomiesChampion => new(
            Title: "🏃 Olympic Zoomies Gold Medalist",
            Tagline: "MUST. RUN. NOW. REASON? UNCLEAR.",
            Description: "At exactly 3:47 AM, or immediately after a bath, or sometimes just BECAUSE, this cavoodle transforms into a furry rocket ship. Furniture is merely an obstacle course. The path must include at least 47 laps of the living room and one dramatic slide across hardwood floors.",
            Strengths: ["Unlimited energy reserves", "Can run on any surface at any angle", "Professional entertainment"],
            Weaknesses: ["Zero impulse control", "RIP to anything breakable at tail height"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be The Flash on a sugar high",
            RarityLevel: "Very Common",
            Emoji: "🏃",
            TraitScores: new(Energy: 5, Floof: 3, Sass: 3, Cuddle: 2, Chaos: 5)
        ),
        
        PersonalityType.PerpetualNapper => new(
            Title: "😴 Napping is a Lifestyle, Not a Hobby",
            Tagline: "Do not disturb. Or do. I won't wake up anyway.",
            Description: "This floofy philosopher has achieved what humans can only dream of: the ability to sleep 18 hours a day without guilt. Every location is a potential nap spot. They've been known to fall asleep mid-chew, mid-walk, and once, legendarily, mid-bark.",
            Strengths: ["Will never destroy furniture (too tired)", "Perfect reading companion", "Expert blanket warmer"],
            Weaknesses: ["May need to check for signs of life occasionally", "Becomes confused when asked to 'do things'"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be Garfield",
            RarityLevel: "Common",
            Emoji: "😴",
            TraitScores: new(Energy: 1, Floof: 5, Sass: 2, Cuddle: 5, Chaos: 1)
        ),
        
        PersonalityType.TreatNegotiator => new(
            Title: "🍪 Professional Treat Negotiator",
            Tagline: "I did a sit. That's at LEAST two treats minimum.",
            Description: "This cunning entrepreneur has turned treat acquisition into an art form. They know every hiding spot. They've memorized the sound of every treat bag in a 5-mile radius. 'Sit' isn't a command—it's an opening offer in a complex negotiation.",
            Strengths: ["Will actually do tricks (for adequate compensation)", "Highly motivated learner", "Excellent memory (for treat-related things only)"],
            Weaknesses: ["Knows you're a pushover", "Has probably already found the secret stash"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be a tiny, fluffy Gordon Gecko",
            RarityLevel: "Extremely Common",
            Emoji: "🍪",
            TraitScores: new(Energy: 4, Floof: 3, Sass: 5, Cuddle: 3, Chaos: 3)
        ),
        
        PersonalityType.FloorFoodInspector => new(
            Title: "🔍 Quality Control Officer: Floor Division",
            Tagline: "If it hit the floor, it's legally mine.",
            Description: "With reflexes that would make a cobra jealous, this cavoodle has never let a dropped crumb touch the ground for more than 0.2 seconds. They've developed sonar specifically tuned to the frequency of food falling. The floor is their jurisdiction, and they take their job VERY seriously.",
            Strengths: ["Your kitchen floor has never been cleaner", "Excellent reflexes", "Will save you from eating that thing you dropped"],
            Weaknesses: ["Has eaten things that were definitely not food", "Cannot distinguish 'edible' from 'absolutely should not eat'"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be a health inspector but for floors",
            RarityLevel: "Common",
            Emoji: "🔍",
            TraitScores: new(Energy: 4, Floof: 3, Sass: 3, Cuddle: 2, Chaos: 4)
        ),
        
        PersonalityType.SockCollector => new(
            Title: "🧦 Sock Acquisition Specialist",
            Tagline: "One sock from every pair. Those are the rules.",
            Description: "Somewhere in your home exists a secret sock hoard of impressive proportions. This fluffy kleptomaniac specifically targets the LEFT sock (always the left, never explained why). They don't chew them—they're not monsters—they simply... relocate them to a secure facility.",
            Strengths: ["Excellent at hide and seek", "Forces you to buy new socks regularly (supporting the economy)", "Very soft crimes"],
            Weaknesses: ["Your sock drawer is a crime scene", "Will 100% blame the cat"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be Carmen Sandiego",
            RarityLevel: "Very Common",
            Emoji: "🧦",
            TraitScores: new(Energy: 3, Floof: 4, Sass: 4, Cuddle: 3, Chaos: 4)
        ),
        
        PersonalityType.ShoeGuardian => new(
            Title: "👟 Designated Shoe Protector",
            Tagline: "These shoes smell like you, therefore I must sleep on them.",
            Description: "Each pair of shoes in your home now comes with a small, fluffy security detail. This cavoodle has claimed all footwear as their personal property. They prefer to either sleep WITH their head IN the shoe, or dramatically present a shoe to you every time you come home. It's their love language.",
            Strengths: ["Shoes always warm when you put them on", "Excellent home security (for shoe-related threats)", "Very devoted"],
            Weaknesses: ["Shoes may contain fur", "Getting dressed takes 10 minutes longer due to negotiations"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be Imelda Marcos's spirit animal",
            RarityLevel: "Common",
            Emoji: "👟",
            TraitScores: new(Energy: 2, Floof: 4, Sass: 3, Cuddle: 4, Chaos: 2)
        ),
        
        PersonalityType.DramaQueen => new(
            Title: "🎭 Dramatic Arts Major",
            Tagline: "Excuse me, I was MILDLY inconvenienced.",
            Description: "The Academy has overlooked this four-legged thespian for too long. The heavy SIGHS when asked to move. The betrayed stare when dinner is 3 minutes late. The full-body collapse when told 'no'. This cavoodle feels emotions at 500% volume and needs you to KNOW about it.",
            Strengths: ["Never boring", "Very expressive communication", "Will let you know EXACTLY how they feel"],
            Weaknesses: ["May call RSPCA if you cut their nails", "Holds grudges FOREVER"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be any Real Housewife",
            RarityLevel: "Very Common",
            Emoji: "🎭",
            TraitScores: new(Energy: 3, Floof: 4, Sass: 5, Cuddle: 3, Chaos: 4)
        ),
        
        PersonalityType.VelcroVelvet => new(
            Title: "🧲 Velcro Dog Extraordinaire",
            Tagline: "Personal space? Never heard of it.",
            Description: "This cavoodle has eliminated the concept of 'alone time' from your vocabulary. Going to the bathroom? They're coming. Taking a shower? They're watching. Sitting down? They're already on you. They're not clingy—they're just conducting very important research on your whereabouts at all times.",
            Strengths: ["Never lonely", "Built-in heated blanket", "Excellent emotional support"],
            Weaknesses: ["You cannot sneeze without a concerned wet nose investigating", "Privacy is a distant memory"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be a loving but slightly concerning stalker",
            RarityLevel: "Extremely Common",
            Emoji: "🧲",
            TraitScores: new(Energy: 3, Floof: 5, Sass: 2, Cuddle: 5, Chaos: 2)
        ),
        
        PersonalityType.GardenArchitect => new(
            Title: "🌱 Landscape Remodeling Consultant",
            Tagline: "Your garden needed more holes. You're welcome.",
            Description: "This cavoodle has a vision for your backyard and it involves CRATERS. Every plant is a potential excavation site. Every garden bed is simply unfinished landscaping. They're not destroying the garden—they're improving it according to their proprietary Dog Feng Shui methodology.",
            Strengths: ["Excellent at digging (if you need holes)", "Never bored outdoors", "Very thorough detector of buried things"],
            Weaknesses: ["RIP to your lawn", "Will emerge from explorations looking like a different colored dog"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be Bob the Builder but chaotic",
            RarityLevel: "Common",
            Emoji: "🌱",
            TraitScores: new(Energy: 5, Floof: 3, Sass: 3, Cuddle: 2, Chaos: 5)
        ),
        
        PersonalityType.SquirrelIntelligence => new(
            Title: "🐿️ Counter-Squirrel Operations Director",
            Tagline: "Trust no one. Especially squirrels.",
            Description: "The squirrels are planning something. This cavoodle KNOWS it. 24/7 surveillance is not paranoia—it's preparedness. Every window is a watchpost. Every tree is suspicious. They've compiled extensive dossiers on every squirrel in the neighborhood and YES they can tell them apart.",
            Strengths: ["Excellent security system", "Never misses a squirrel", "Terrifying to small wildlife"],
            Weaknesses: ["Cannot focus on anything when squirrel is visible", "May attempt to go through glass to reach target"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be in the FBI",
            RarityLevel: "Common",
            Emoji: "🐿️",
            TraitScores: new(Energy: 4, Floof: 3, Sass: 4, Cuddle: 2, Chaos: 4)
        ),
        
        PersonalityType.WindowWatcher => new(
            Title: "🪟 Neighborhood Watch Commander",
            Tagline: "That leaf looked at me wrong.",
            Description: "This vigilant patrol officer has taken it upon themselves to protect the household from ALL threats. Leaves. Plastic bags. The audacity of people walking on the sidewalk. Someone three blocks away closing a car door. All must be barked at. All must be warned.",
            Strengths: ["You'll never be surprised by a delivery person", "Excellent hearing", "Very alert"],
            Weaknesses: ["Will bark at literal air", "Believes they have prevented 847 invasions that were actually just wind"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be a retired mall cop",
            RarityLevel: "Very Common",
            Emoji: "🪟",
            TraitScores: new(Energy: 3, Floof: 3, Sass: 4, Cuddle: 2, Chaos: 4)
        ),
        
        PersonalityType.DoorGreeter => new(
            Title: "🚪 Chief Happiness Officer",
            Tagline: "YOU'RE HOME! YOU WERE GONE FOR 84 YEARS!",
            Description: "Whether you were gone for 8 hours or 8 seconds, this cavoodle greets you like a soldier returning from war. The whole-body wiggles. The spinning. The inability to contain joy in their small fluffy body. Every arrival is the BEST DAY OF THEIR ENTIRE LIFE.",
            Strengths: ["Will make you feel like a celebrity", "Unlimited enthusiasm", "Pure serotonin in dog form"],
            Weaknesses: ["May injure themselves from excitement", "Takes 15 minutes to calm down enough to function"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be that one friend who's always happy to see you",
            RarityLevel: "Very Common",
            Emoji: "🚪",
            TraitScores: new(Energy: 5, Floof: 4, Sass: 1, Cuddle: 5, Chaos: 3)
        ),
        
        _ => new(
            Title: "🐕 Mystery Floof",
            Tagline: "An enigma wrapped in fluff.",
            Description: "This cavoodle defies classification. They contain multitudes.",
            Strengths: ["Unpredictable", "Full of surprises"],
            Weaknesses: ["Literally no one knows what they'll do next"],
            CelebMatch: "If your cavoodle was a celebrity, they'd be a mysterious A-lister",
            RarityLevel: "Legendary",
            Emoji: "🐕",
            TraitScores: new(Energy: 3, Floof: 3, Sass: 3, Cuddle: 3, Chaos: 3)
        )
    };
    
    public record TraitScores(int Energy, int Floof, int Sass, int Cuddle, int Chaos);
    
    public record PersonalityInfo(
        string Title,
        string Tagline,
        string Description,
        string[] Strengths,
        string[] Weaknesses,
        string CelebMatch,
        string RarityLevel,
        string Emoji,
        TraitScores TraitScores
    );
}
