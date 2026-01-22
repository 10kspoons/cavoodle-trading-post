using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using CavoodleTrading.Api.Domain.Entities;
using CavoodleTrading.Api.Domain.Enums;
using CavoodleTrading.Api.Infrastructure.Data;

namespace CavoodleTrading.Api.Features.Listings;

public static class ListingsEndpoints
{
    public static void MapListingsEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/listings").WithTags("Listings");

        group.MapGet("/", GetListings);
        group.MapGet("/{id:guid}", GetListing);
        group.MapPost("/", CreateListing).RequireAuthorization();
        group.MapPut("/{id:guid}", UpdateListing).RequireAuthorization();
        group.MapDelete("/{id:guid}", DeleteListing).RequireAuthorization();
        group.MapGet("/my", GetMyListings).RequireAuthorization();
    }

    private static async Task<IResult> GetListings(
        CavoodleDbContext db,
        string? color = null,
        string? state = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        int page = 1,
        int pageSize = 20)
    {
        var query = db.Listings
            .Include(l => l.Photos.Where(p => p.IsPrimary))
            .Include(l => l.PersonalityProfile)
            .Include(l => l.Seller)
            .Where(l => l.Status == ListingStatus.Active)
            .AsQueryable();

        if (!string.IsNullOrEmpty(color) && Enum.TryParse<CavoodleColor>(color, true, out var colorEnum))
        {
            query = query.Where(l => l.Color == colorEnum);
        }

        if (!string.IsNullOrEmpty(state))
        {
            query = query.Where(l => l.State.ToLower() == state.ToLower());
        }

        if (minPrice.HasValue)
        {
            query = query.Where(l => l.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(l => l.Price <= maxPrice.Value);
        }

        var totalCount = await query.CountAsync();
        
        var listings = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new ListingDto
            {
                Id = l.Id,
                Name = l.Name,
                AgeMonths = l.AgeMonths,
                Color = l.Color.ToString(),
                Gender = l.Gender.ToString(),
                Price = l.Price,
                Suburb = l.Suburb,
                State = l.State,
                PrimaryPhotoUrl = l.Photos.FirstOrDefault(p => p.IsPrimary) != null 
                    ? l.Photos.First(p => p.IsPrimary).Url 
                    : null,
                PersonalityType = l.PersonalityProfile != null 
                    ? l.PersonalityProfile.Type.ToString() 
                    : null,
                FloofFactor = l.PersonalityProfile != null ? l.PersonalityProfile.FloofFactor : 0,
                SellerName = l.Seller.DisplayName,
                SellerKarma = l.Seller.CavoodleKarma,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Results.Ok(new 
        { 
            Items = listings, 
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
        });
    }

    private static async Task<IResult> GetListing(Guid id, CavoodleDbContext db)
    {
        var listing = await db.Listings
            .Include(l => l.Photos.OrderBy(p => p.DisplayOrder))
            .Include(l => l.PersonalityProfile)
            .Include(l => l.Seller)
            .FirstOrDefaultAsync(l => l.Id == id);

        if (listing == null)
        {
            return Results.NotFound();
        }

        return Results.Ok(new ListingDetailDto
        {
            Id = listing.Id,
            Name = listing.Name,
            AgeMonths = listing.AgeMonths,
            Color = listing.Color.ToString(),
            Gender = listing.Gender.ToString(),
            Price = listing.Price,
            Description = listing.Description,
            Suburb = listing.Suburb,
            State = listing.State,
            Status = listing.Status.ToString(),
            Photos = listing.Photos.Select(p => new PhotoDto { Url = p.Url, IsPrimary = p.IsPrimary }).ToList(),
            Personality = listing.PersonalityProfile != null ? new PersonalityDto
            {
                Type = listing.PersonalityProfile.Type.ToString(),
                TypeDisplayName = GetPersonalityDisplayName(listing.PersonalityProfile.Type),
                EnergyLevel = listing.PersonalityProfile.EnergyLevel,
                FloofFactor = listing.PersonalityProfile.FloofFactor,
                SassRating = listing.PersonalityProfile.SassRating,
                CustomBio = listing.PersonalityProfile.CustomBio
            } : null,
            Seller = new SellerDto
            {
                Id = listing.Seller.Id,
                DisplayName = listing.Seller.DisplayName,
                AvatarUrl = listing.Seller.AvatarUrl,
                CavoodleKarma = listing.Seller.CavoodleKarma
            },
            CreatedAt = listing.CreatedAt
        });
    }

    private static async Task<IResult> CreateListing(
        CreateListingRequest request,
        ClaimsPrincipal principal,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var listing = new CavoodleListing
        {
            Id = Guid.NewGuid(),
            SellerId = userId,
            Name = request.Name,
            AgeMonths = request.AgeMonths,
            Color = Enum.Parse<CavoodleColor>(request.Color, true),
            Gender = Enum.Parse<CavoodleGender>(request.Gender, true),
            Price = request.Price,
            Description = request.Description,
            Suburb = request.Suburb,
            State = request.State,
            Status = ListingStatus.Active,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        db.Listings.Add(listing);
        await db.SaveChangesAsync();

        return Results.Created($"/api/listings/{listing.Id}", new { Id = listing.Id });
    }

    private static async Task<IResult> UpdateListing(
        Guid id,
        UpdateListingRequest request,
        ClaimsPrincipal principal,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.SellerId == userId);
        
        if (listing == null)
        {
            return Results.NotFound();
        }

        listing.Name = request.Name ?? listing.Name;
        listing.AgeMonths = request.AgeMonths ?? listing.AgeMonths;
        listing.Price = request.Price ?? listing.Price;
        listing.Description = request.Description ?? listing.Description;
        listing.Suburb = request.Suburb ?? listing.Suburb;
        listing.State = request.State ?? listing.State;
        listing.UpdatedAt = DateTime.UtcNow;

        if (!string.IsNullOrEmpty(request.Status) && Enum.TryParse<ListingStatus>(request.Status, true, out var status))
        {
            listing.Status = status;
        }

        await db.SaveChangesAsync();

        return Results.Ok(new { Id = listing.Id });
    }

    private static async Task<IResult> DeleteListing(
        Guid id,
        ClaimsPrincipal principal,
        CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var listing = await db.Listings.FirstOrDefaultAsync(l => l.Id == id && l.SellerId == userId);
        
        if (listing == null)
        {
            return Results.NotFound();
        }

        db.Listings.Remove(listing);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static async Task<IResult> GetMyListings(ClaimsPrincipal principal, CavoodleDbContext db)
    {
        var userId = Guid.Parse(principal.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var listings = await db.Listings
            .Include(l => l.Photos.Where(p => p.IsPrimary))
            .Where(l => l.SellerId == userId)
            .OrderByDescending(l => l.CreatedAt)
            .Select(l => new ListingDto
            {
                Id = l.Id,
                Name = l.Name,
                AgeMonths = l.AgeMonths,
                Color = l.Color.ToString(),
                Gender = l.Gender.ToString(),
                Price = l.Price,
                Suburb = l.Suburb,
                State = l.State,
                Status = l.Status.ToString(),
                PrimaryPhotoUrl = l.Photos.FirstOrDefault(p => p.IsPrimary) != null 
                    ? l.Photos.First(p => p.IsPrimary).Url 
                    : null,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync();

        return Results.Ok(listings);
    }

    private static string GetPersonalityDisplayName(PersonalityType type) => type switch
    {
        PersonalityType.CouchThief => "🛋️ Professional Couch Thief",
        PersonalityType.CushionEngineer => "🏗️ Certified Cushion Engineer",
        PersonalityType.ZoomiesChampion => "🏃 Olympic Zoomies Gold Medalist",
        PersonalityType.PerpetualNapper => "😴 Napping is a Lifestyle",
        PersonalityType.TreatNegotiator => "🍪 Professional Treat Negotiator",
        PersonalityType.FloorFoodInspector => "🔍 Floor Quality Control Officer",
        PersonalityType.SockCollector => "🧦 Sock Acquisition Specialist",
        PersonalityType.ShoeGuardian => "👟 Designated Shoe Protector",
        PersonalityType.DramaQueen => "🎭 Dramatic Arts Major",
        PersonalityType.VelcroVelvet => "🧲 Velcro Dog Extraordinaire",
        PersonalityType.GardenArchitect => "🌱 Landscape Remodeling Consultant",
        PersonalityType.SquirrelIntelligence => "🐿️ Counter-Squirrel Operations Director",
        PersonalityType.WindowWatcher => "🪟 Neighborhood Watch Commander",
        PersonalityType.DoorGreeter => "🚪 Chief Happiness Officer",
        _ => type.ToString()
    };
}

// DTOs
public record ListingDto
{
    public Guid Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int AgeMonths { get; init; }
    public string Color { get; init; } = string.Empty;
    public string Gender { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Suburb { get; init; } = string.Empty;
    public string State { get; init; } = string.Empty;
    public string? Status { get; init; }
    public string? PrimaryPhotoUrl { get; init; }
    public string? PersonalityType { get; init; }
    public int FloofFactor { get; init; }
    public string? SellerName { get; init; }
    public int SellerKarma { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record ListingDetailDto : ListingDto
{
    public string Description { get; init; } = string.Empty;
    public List<PhotoDto> Photos { get; init; } = new();
    public PersonalityDto? Personality { get; init; }
    public SellerDto Seller { get; init; } = null!;
}

public record PhotoDto
{
    public string Url { get; init; } = string.Empty;
    public bool IsPrimary { get; init; }
}

public record PersonalityDto
{
    public string Type { get; init; } = string.Empty;
    public string TypeDisplayName { get; init; } = string.Empty;
    public int EnergyLevel { get; init; }
    public int FloofFactor { get; init; }
    public int SassRating { get; init; }
    public string? CustomBio { get; init; }
}

public record SellerDto
{
    public Guid Id { get; init; }
    public string DisplayName { get; init; } = string.Empty;
    public string? AvatarUrl { get; init; }
    public int CavoodleKarma { get; init; }
}

public record CreateListingRequest(
    string Name,
    int AgeMonths,
    string Color,
    string Gender,
    decimal Price,
    string Description,
    string Suburb,
    string State
);

public record UpdateListingRequest(
    string? Name = null,
    int? AgeMonths = null,
    decimal? Price = null,
    string? Description = null,
    string? Suburb = null,
    string? State = null,
    string? Status = null
);
