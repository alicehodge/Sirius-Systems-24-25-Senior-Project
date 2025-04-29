using Microsoft.EntityFrameworkCore;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using StorkDorkMain.Models;

namespace StorkDorkMain.DAL.Concrete
{
    public class NotificationRepository : Repository<Notification>, INotificationRepository
    {
        private readonly StorkDorkContext _context;
        private DbSet<Notification> _notifications;

        public NotificationRepository(StorkDorkContext context) : base(context)
        {
            _context = context;
            _notifications = context.Notifications;
        }

        public async Task<List<Notification>> GetRecentByUserIdAsync(int userId, int take = 20)
        {
            return await _notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetUnreadCountForUserAsync(int userId)
        {
            return await _notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .CountAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}