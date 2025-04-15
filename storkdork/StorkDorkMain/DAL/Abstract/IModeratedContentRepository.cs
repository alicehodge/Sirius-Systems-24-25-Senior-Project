using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Abstract
{
    public interface IModeratedContentRepository : IRepository<ModeratedContent>
    {
        Task<IEnumerable<ModeratedContent>> GetContentWithSubmitterAsync(string status);
        Task<ModeratedContent> GetContentWithDetailsAsync(int id);
        Task SaveChangesAsync();
        void AddOrUpdate(ModeratedContent content);
    }
}