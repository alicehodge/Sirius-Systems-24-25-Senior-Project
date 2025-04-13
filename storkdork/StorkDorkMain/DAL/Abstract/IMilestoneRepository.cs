using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Abstract;

public interface IMilestoneRepository : IRepository<Milestone>
{
    Task<int> GetSightingsMade(int userId);
    Task<int> GetPhotosContributed(int userId);
    Task<MostSpottedBirdDTO?> GetMostSpottedBirdAsync(int userId);
    Task<SightingsInADayDTO?> GetDailySightingRecord(int userId);
    int GetMilestoneTier(int Achievement);
    void IncrementSightingsMade(int userId);
}