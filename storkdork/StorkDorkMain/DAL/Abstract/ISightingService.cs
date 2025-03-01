using StorkDorkMain.Models;
using StorkDorkMain.Models.DTO;

namespace StorkDorkMain.DAL.Abstract;

public interface ISightingService
{
    Task<List<SightMarker>> GetSightingsAsync();
    Task<List<SightMarker>> GetSightingsByUserIdAsync(int userId);
    // Task<List<SightMarker>> GetSightingsByCurrentUserIdAsync(int userId);
}