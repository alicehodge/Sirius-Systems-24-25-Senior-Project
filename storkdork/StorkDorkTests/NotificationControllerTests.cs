using NUnit.Framework;
using Moq;
using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Controllers;
using StorkDorkMain.Services;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace StorkDorkMain.Tests.Controllers
{
    [TestFixture]
    public class NotificationsControllerTests : IDisposable
    {
        private Mock<INotificationService> _mockNotificationService;
        private Mock<ISDUserRepository> _mockUserRepository;
        private Mock<ILogger<NotificationsController>> _mockLogger;
        private NotificationsController _controller;
        private SdUser _testUser;
        private List<Notification> _testNotifications;

        [SetUp]
        public void Setup()
        {
            _mockNotificationService = new Mock<INotificationService>();
            _mockUserRepository = new Mock<ISDUserRepository>();
            _mockLogger = new Mock<ILogger<NotificationsController>>();
            _controller = new NotificationsController(
                _mockNotificationService.Object,
                _mockUserRepository.Object,
                _mockLogger.Object
            );

            // Setup test data
            _testUser = new SdUser { Id = 1, DisplayName = "Test User" };
            _testNotifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = 1, Message = "Test 1", IsRead = false },
                new Notification { Id = 2, UserId = 1, Message = "Test 2", IsRead = true }
            };

            // Setup default user claims
            var claims = new List<Claim> { new Claim(ClaimTypes.NameIdentifier, "test-user") };
            var identity = new ClaimsIdentity(claims);
            var principal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = principal }
            };
        }

        [Test]
        public async Task Index_UserExists_ReturnsViewWithNotifications()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);
            _mockNotificationService.Setup(s => s.GetUserNotificationsAsync(_testUser.Id))
                .ReturnsAsync(_testNotifications);

            // Act
            var result = await _controller.Index() as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Model, Is.EqualTo(_testNotifications));
        }

        [Test]
        public async Task Index_UserNotFound_ReturnsChallengeResult()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((SdUser)null);

            // Act
            var result = await _controller.Index();

            // Assert
            Assert.That(result, Is.InstanceOf<ChallengeResult>());
        }

        [Test]
        public async Task ToggleRead_Success_ReturnsJsonSuccess()
        {
            // Arrange
            _mockNotificationService.Setup(s => s.ToggleReadStatusAsync(It.IsAny<int>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.ToggleRead(1) as JsonResult;
            var jsonString = JsonSerializer.Serialize(result?.Value);
            var response = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(response["success"].GetBoolean(), Is.True);
        }

        [Test]
        public async Task ToggleRead_Exception_ReturnsJsonFailure()
        {
            // Arrange
            _mockNotificationService.Setup(s => s.ToggleReadStatusAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.ToggleRead(1) as JsonResult;
            var jsonString = JsonSerializer.Serialize(result?.Value);
            var response = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(jsonString);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(response["success"].GetBoolean(), Is.False);
        }

        [Test]
        public async Task GetUnreadCount_UserExists_ReturnsCount()
        {
            // Arrange
            const int expectedCount = 5;
            _mockUserRepository.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);
            _mockNotificationService.Setup(s => s.GetUnreadCountAsync(_testUser.Id))
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _controller.GetUnreadCount() as JsonResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value, Is.EqualTo(expectedCount));
        }

        [Test]
        public async Task GetUnreadCount_UserNotFound_ReturnsUnauthorized()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((SdUser)null);

            // Act
            var result = await _controller.GetUnreadCount();

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        [Test]
        public async Task GetUnreadCount_Exception_Returns500()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(_testUser);
            _mockNotificationService.Setup(s => s.GetUnreadCountAsync(It.IsAny<int>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _controller.GetUnreadCount() as StatusCodeResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(500));
        }

        [TearDown]
        public void Dispose()
        {
            _controller?.Dispose();
            _mockNotificationService?.Reset();
            _mockUserRepository?.Reset();
            _mockLogger?.Reset();
            _testUser = null;
            _testNotifications = null;
        }

        void IDisposable.Dispose()
        {
            Dispose();
        }
    }
}