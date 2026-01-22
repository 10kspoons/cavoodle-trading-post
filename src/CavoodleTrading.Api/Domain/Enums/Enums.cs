namespace CavoodleTrading.Api.Domain.Enums;

public enum PersonalityType
{
    CouchThief,           // "Professional Couch Thief"
    ZoomiesChampion,      // "Olympic Zoomies Champion"
    TreatNegotiator,      // "Professional Treat Negotiator"
    SockCollector,        // "Certified Sock Collector"
    BarkingBarrister,     // "Barking Barrister" (barks at everything)
    VelcroVelvet,         // "Velcro Dog Extraordinaire"
    GardenDestroyer,      // "Landscape Architect" (digs holes)
    DramaQueen            // "Dramatic Arts Major"
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
    Phantom,      // Two-tone
    Parti,        // Multi-color patches
    Merle
}

public enum CavoodleGender
{
    Male,
    Female
}
