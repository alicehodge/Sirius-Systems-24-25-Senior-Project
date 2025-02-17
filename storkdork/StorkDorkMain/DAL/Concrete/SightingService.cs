using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using StorkDorkMain.Models.DTO;

namespace StorkDorkMain.DAL.Concrete;

public class SightingService : ISightingService
{
    private readonly StorkDorkContext _context;

    public SightingService(StorkDorkContext context)
    {
        _context = context;
    }

    public async Task<List<SightMarker>> GetSightingsAsync()
    {
        return await _context.Sightings
            .Include(s => s.Bird)
            .Select(s => new SightMarker
            {
                CommonName = s.Bird.CommonName ?? "Unkown Name",
                SciName = s.Bird.ScientificName,
                Longitude = s.Longitude,
                Latitude = s.Latitude,
                Description = s.Notes,
                Date = s.Date
            })
            .ToListAsync();
    }
}