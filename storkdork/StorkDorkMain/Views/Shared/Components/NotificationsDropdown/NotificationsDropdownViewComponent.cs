using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Services;
using StorkDorkMain.Models;

namespace StorkDorkMain.ViewComponents
{
    public class NotificationsDropdownViewComponent : ViewComponent
    {
        private readonly INotificationService _notificationService;
        private readonly ISDUserRepository _userRepository;

        public NotificationsDropdownViewComponent(
            INotificationService notificationService,
            ISDUserRepository userRepository)
        {
            _notificationService = notificationService;
            _userRepository = userRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userRepository.GetSDUserByIdentity(HttpContext.User);
            if (user == null)
            {
                return Content("");
            }

            var notifications = await _notificationService.GetUserNotificationsAsync(user.Id);
            ViewBag.UnreadCount = await _notificationService.GetUnreadCountAsync(user.Id);
            
            return View("Default", notifications);
        }
    }
}