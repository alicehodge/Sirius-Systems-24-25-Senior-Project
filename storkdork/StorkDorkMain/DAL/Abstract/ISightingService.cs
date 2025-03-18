using StorkDorkMain.Models;
using StorkDorkMain.Models.DTO;

namespace StorkDorkMain.DAL.Abstract;

public interface ISightingService
{
    Task<List<SightMarker>> GetSightingsAsync();
    Task<List<SightMarker>> GetSightingsByUserIdAsync(int userId);
    Task<IEnumerable<Sighting>> GetNearestSightingsForBird(int birdId, double? lat = null, double? lng = null);
    Task UpdateSightingLocationAsync(int sightingId, string country, string subdivision);

    // Task<List<SightMarker>> GetSightingsByCurrentUserIdAsync(int userId);
}