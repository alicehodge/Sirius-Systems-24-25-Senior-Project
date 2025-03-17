using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using StorkDorkMain.Models.DTO;
using StorkDork.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace StorkDorkMain.DAL.Concrete;

public class SightingService : ISightingService
{
    private readonly StorkDorkContext _storkDorkContext;
    private readonly UserManager<IdentityUser> _usermanager;

    public SightingService(StorkDorkContext storkDorkContext, UserManager<IdentityUser> userManager)
    {
        _storkDorkContext = storkDorkContext;
        _usermanager = userManager;
    }

    public async Task<List<SightMarker>> GetSightingsAsync()
    {
        return await _storkDorkContext.Sightings
            .Include(s => s.Bird)
            .Select(s => new SightMarker
            {
                SightingId = s.Id,
                CommonName = s.Bird.CommonName ?? "Unkown Name",
                SciName = s.Bird.ScientificName,
                Longitude = s.Longitude,
                Latitude = s.Latitude,
                Description = s.Notes,
                Date = s.Date,
                Country = s.Country,
                Subdivision = s. Subdivision
            })
            .ToListAsync();
    }

    public async Task<List<SightMarker>> GetSightingsByUserIdAsync(int userId)
    {
        return await _storkDorkContext.Sightings
            .Where(s => s.SdUserId == userId)
            .Include(s => s.Bird)
            .Select(s => new SightMarker
            {
                SightingId = s.Id,
                CommonName = s.Bird.CommonName ?? "Unkown Name",
                SciName = s.Bird.ScientificName,
                Longitude = s.Longitude,
                Latitude = s.Latitude,
                Description = s.Notes,
                Date = s.Date,
                Country = s.Country,
                Subdivision = s.Subdivision
            })
            .ToListAsync();
    }

    public async Task UpdateSightingLocationAsync(int sightingId, string country, string subdivision)
    {
        var sighting = await _storkDorkContext.Sightings.FindAsync(sightingId);
        if (sighting == null)
        {
            throw new KeyNotFoundException("Sighting not found.");
        }

        sighting.Country = country;
        sighting.Subdivision = subdivision;
        await _storkDorkContext.SaveChangesAsync();
    }

    // public async Task<List<SightMarker>> GetSightingsByCurrentUserAsync(int userId)
    // {
    //     string userID = _usermanager.GetUserId(User);
    //     return await 
    // }
}