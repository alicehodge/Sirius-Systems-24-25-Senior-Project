using StorkDorkMain.Models;
using StorkDorkMain.DAL.Abstract;
using Microsoft.Extensions.Logging;

namespace StorkDorkMain.Services
{
    public interface INotificationService
    {
        Task CreateNotificationAsync(int userId, string message, string type, string? relatedUrl = null);
        Task<List<Notification>> GetUserNotificationsAsync(int userId);
        Task MarkAsReadAsync(int notificationId);
        Task<int> GetUnreadCountAsync(int userId);
    }

    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            INotificationRepository notificationRepository,
            ILogger<NotificationService> logger)
        {
            _notificationRepository = notificationRepository;
            _logger = logger;
        }

        public async Task CreateNotificationAsync(int userId, string message, string type, string? relatedUrl = null)
        {
            try
            {
                var notification = new Notification
                {
                    UserId = userId,
                    Message = message,
                    Type = type,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow,
                    RelatedUrl = relatedUrl
                };

                _notificationRepository.AddOrUpdate(notification);
                await _notificationRepository.SaveChangesAsync();

                _logger.LogInformation($"Created notification for user {userId}: {message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating notification for user {userId}");
                throw;
            }
        }

        public async Task<List<Notification>> GetUserNotificationsAsync(int userId)
        {
            try
            {
                return await _notificationRepository.GetRecentByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving notifications for user {userId}");
                throw;
            }
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            try
            {
                var notification = _notificationRepository.FindById(notificationId);
                if (notification != null)
                {
                    notification.IsRead = true;
                    _notificationRepository.AddOrUpdate(notification);
                    await _notificationRepository.SaveChangesAsync();
                    _logger.LogInformation($"Marked notification {notificationId} as read");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error marking notification {notificationId} as read");
                throw;
            }
        }

        public async Task<int> GetUnreadCountAsync(int userId)
        {
            try
            {
                return await _notificationRepository.GetUnreadCountForUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting unread count for user {userId}");
                throw;
            }
        }
    }
}