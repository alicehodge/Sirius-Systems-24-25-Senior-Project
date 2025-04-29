using NUnit.Framework;
using Moq;
using StorkDorkMain.Services;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using Microsoft.Extensions.Logging;

namespace StorkDorkMain.Tests.Services
{
    [TestFixture]
    public class NotificationServiceTests
    {
        private Mock<INotificationRepository> _mockRepository;
        private Mock<ILogger<NotificationService>> _mockLogger;
        private NotificationService _service;
        private List<Notification> _testNotifications;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<INotificationRepository>();
            _mockLogger = new Mock<ILogger<NotificationService>>();
            _service = new NotificationService(_mockRepository.Object, _mockLogger.Object);

            _testNotifications = new List<Notification>
            {
                new Notification { Id = 1, UserId = 1, Message = "Test 1", IsRead = false },
                new Notification { Id = 2, UserId = 1, Message = "Test 2", IsRead = true }
            };
        }

        [Test]
        public async Task CreateNotificationAsync_Success_CreatesNotification()
        {
            // Arrange
            const int userId = 1;
            const string message = "Test notification";
            const string type = "Info";
            Notification capturedNotification = null;

            _mockRepository.Setup(r => r.AddOrUpdate(It.IsAny<Notification>()))
                .Callback<Notification>(n => capturedNotification = n);

            // Act
            await _service.CreateNotificationAsync(userId, message, type);

            // Assert
            Assert.That(capturedNotification, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(capturedNotification.UserId, Is.EqualTo(userId));
                Assert.That(capturedNotification.Message, Is.EqualTo(message));
                Assert.That(capturedNotification.Type, Is.EqualTo(type));
                Assert.That(capturedNotification.IsRead, Is.False);
            });
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetUserNotificationsAsync_Success_ReturnsNotifications()
        {
            // Arrange
            const int userId = 1;
            _mockRepository.Setup(r => r.GetRecentByUserIdAsync(userId, It.IsAny<int>()))
                .ReturnsAsync(_testNotifications);

            // Act
            var result = await _service.GetUserNotificationsAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(_testNotifications));
        }

        [Test]
        public async Task ToggleReadStatusAsync_Success_TogglesStatus()
        {
            // Arrange
            var notification = _testNotifications[0];
            var originalIsRead = notification.IsRead;
            
            _mockRepository.Setup(r => r.FindById(notification.Id))
                .Returns(notification);

            // Act
            await _service.ToggleReadStatusAsync(notification.Id);

            // Assert
            Assert.That(notification.IsRead, Is.Not.EqualTo(originalIsRead));
            _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Test]
        public async Task GetUnreadCountAsync_Success_ReturnsCount()
        {
            // Arrange
            const int userId = 1;
            const int expectedCount = 5;
            _mockRepository.Setup(r => r.GetUnreadCountForUserAsync(userId))
                .ReturnsAsync(expectedCount);

            // Act
            var result = await _service.GetUnreadCountAsync(userId);

            // Assert
            Assert.That(result, Is.EqualTo(expectedCount));
        }

        [Test]
        public void CreateNotificationAsync_Exception_Throws()
        {
            // Arrange
            _mockRepository.Setup(r => r.AddOrUpdate(It.IsAny<Notification>()))
                .Throws<Exception>();

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => 
                _service.CreateNotificationAsync(1, "Test", "Info"));
        }

        [Test]
        public void GetUserNotificationsAsync_Exception_Throws()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetRecentByUserIdAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ThrowsAsync(new Exception());

            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => 
                _service.GetUserNotificationsAsync(1));
        }
    }
}