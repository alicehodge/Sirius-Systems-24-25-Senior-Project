using StorkDorkMain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace StorkDorkMain.DAL.Abstract
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<List<Notification>> GetRecentByUserIdAsync(int userId, int take = 20);
        Task<int> GetUnreadCountForUserAsync(int userId);
        Task SaveChangesAsync();    
    }
}