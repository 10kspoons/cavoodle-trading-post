namespace CavoodleTrading.Api.Domain.Enums;

/// <summary>
/// The sacred 12 Cavoodle Personality Types - scientifically determined by 
/// decades of research (and scrolling cavoodle Instagram accounts at 2am)
/// </summary>
public enum PersonalityType
{
    // 🛋️ Couch-Related Personalities
    CouchThief,           // "Professional Couch Thief" - steals your spot the moment you get up
    CushionEngineer,      // "Certified Cushion Engineer" - rearranges all cushions into a nest
    
    // 🏃 Energy Levels
    ZoomiesChampion,      // "Olympic Zoomies Gold Medalist" - 3am zoomies specialist
    PerpetualNapper,      // "Napping is a Lifestyle, Not a Hobby"
    
    // 🍖 Food-Related
    TreatNegotiator,      // "Professional Treat Negotiator" - will do anything for snacks
    FloorFoodInspector,   // "Quality Control Officer: Floor Division"
    
    // 🧦 Collection Hobbies  
    SockCollector,        // "Sock Acquisition Specialist" - one from every pair
    ShoeGuardian,         // "Designated Shoe Protector" - sleeps on/in shoes
    
    // 🎭 Drama & Emotion
    DramaQueen,           // "Dramatic Arts Major" - Oscar-worthy guilt trips
    VelcroVelvet,         // "Velcro Dog Extraordinaire" - personal space? never heard of it
    
    // 🌿 Outdoor Behaviors
    GardenArchitect,      // "Landscape Remodeling Consultant" - digs strategic holes
    SquirrelIntelligence, // "Counter-Squirrel Operations Director" - 24/7 surveillance
    
    // 🪟 Alert Behaviors
    WindowWatcher,        // "Neighborhood Watch Commander" - barks at falling leaves
    DoorGreeter           // "Chief Happiness Officer" - maximum wiggle butts
}

public enum ListingStatus
{
    Draft,
    Active,
    Pending,      // Sale pending
    Sold,
    Withdrawn
}

public enum CavoodleColor
{
    Cream,
    Apricot,
    Gold,
    Red,
    Chocolate,
    Black,
    Phantom,      // Two-tone (black with tan markings)
    Parti,        // Multi-color patches
    Merle,        // Marbled pattern
    Champagne,    // Light golden
    Sable         // Multi-shaded
}

public enum CavoodleGender
{
    Male,
    Female
}
