using StorkDorkMain.Models.DTO;

namespace StorkDorkMain.DAL.Abstract;

public interface ISightingService
{
    Task<List<SightMarker>> GetSightingsAsync();
}