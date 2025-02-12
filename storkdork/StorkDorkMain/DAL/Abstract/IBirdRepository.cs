using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Abstract
{
    public interface IBirdRepository : IRepository<Bird>
    {
        Task<IEnumerable<Bird>> GetBirdsByName(string name);
    }
}