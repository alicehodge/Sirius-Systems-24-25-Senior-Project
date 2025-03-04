using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Abstract
{
    public interface IBirdRepository : IRepository<Bird>
    {
        Task<IEnumerable<Bird>> GetBirdsByName(string name);
        Task<IEnumerable<Bird>> GetBirdsByTaxonomy(string searchTerm);
        Task<(IEnumerable<Bird> Birds, int TotalCount)> GetRelatedBirds(
            string speciesCode, 
            string? reportAs, 
            string? categoryFilter = null,
            int page = 1, 
            int pageSize = 10);
    }
}