using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Services;
using StorkDorkMain.DAL.Abstract;

namespace StorkDorkMain.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly ISDUserRepository _userRepository;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(
            INotificationService notificationService,
            ISDUserRepository userRepository, ILogger<NotificationsController> logger)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userRepository.GetSDUserByIdentity(User);
            if (user == null) return Challenge();

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            return View(notifications);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleRead(int id)
        {
            try
            {
                await _notificationService.ToggleReadStatusAsync(id);
                return Json(new { success = true });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetUnreadCount()
        {
            try
            {
                var user = await _userRepository.GetSDUserByIdentity(User);
                if (user == null) return Unauthorized();

                var count = await _notificationService.GetUnreadCountAsync(user.Id);
                return Json(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting unread notification count");
                return StatusCode(500);
            }
        }
    }
}