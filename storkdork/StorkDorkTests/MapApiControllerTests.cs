using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using StorkDorkMain.Controllers;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models.DTO;

namespace StorkDorkTests
{
    [TestFixture]
    public class MapApiControllerTests
    {
        private Mock<ISightingService> _mockSightingService;
        private MapApiController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockSightingService = new Mock<ISightingService>();
            _controller = new MapApiController(_mockSightingService.Object);
        }

        [Test]
        public async Task GetSightings_ReturnsOk_WithSightings()
        {
            // Arrange
            var mockSightings = new List<SightMarker>
            {
                new SightMarker 
                { 
                    CommonName = "American Herring Gull", 
                    SciName = "Larus smithsonianus",
                    Longitude = -123.24m, 
                    Latitude = 44.85m, 
                    Description = "Seen near the park",
                    Date = DateTime.UtcNow
                },
                new SightMarker 
                { 
                    CommonName = "Northern Flicker", 
                    SciName = "Colaptes auratus",
                    Longitude = -123.25m, 
                    Latitude = 44.86m, 
                    Description = "Perched on a fence",
                    Date = DateTime.UtcNow
                }
            };

            _mockSightingService
                .Setup(s => s.GetSightingsAsync())
                .ReturnsAsync(mockSightings);

            // Act
            var result = await _controller.GetSightings();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(mockSightings));
        }

        [Test]
        public async Task GetSightings_ReturnsOk_WithEmptyList()
        {
            // Arrange
            _mockSightingService
                .Setup(s => s.GetSightingsAsync())
                .ReturnsAsync(new List<SightMarker>());

            // Act
            var result = await _controller.GetSightings();

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.Not.Null);
            Assert.That(okResult.Value, Is.InstanceOf<List<SightMarker>>());
            Assert.That(((List<SightMarker>)okResult.Value).Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSightingsByUserId_ReturnsOk_WithUserSightings()
        {
            // Arrange
            int userId = 1;
            var mockSightings = new List<SightMarker>
            {
                new SightMarker 
                { 
                    CommonName = "Common Raven", 
                    SciName = "Corvus corax",
                    Longitude = -123.2378m, 
                    Latitude = 44.8490m, 
                    Description = "Hopping near the library",
                    Date = DateTime.UtcNow
                }
            };

            _mockSightingService
                .Setup(s => s.GetSightingsByUserIdAsync(userId))
                .ReturnsAsync(mockSightings);

            // Act
            var result = await _controller.GetSightingsByUserId(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.EqualTo(mockSightings));
        }

        [Test]
        public async Task GetSightingsByUserId_ReturnsOk_WithEmptyList()
        {
            // Arrange
            int userId = 99; // Assume this user has no sightings
            _mockSightingService
                .Setup(s => s.GetSightingsByUserIdAsync(userId))
                .ReturnsAsync(new List<SightMarker>());

            // Act
            var result = await _controller.GetSightingsByUserId(userId);

            // Assert
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            var okResult = result as OkObjectResult;
            Assert.That(okResult.Value, Is.InstanceOf<List<SightMarker>>());
            Assert.That(((List<SightMarker>)okResult.Value).Count, Is.EqualTo(0));
        }

        [Test]
        public async Task GetSightingsByUserId_WithInvalidUserId_ReturnsBadRequest()
        {
            // Arrange
            int invalidUserId = -1; // Invalid ID

            // Act
            var result = await _controller.GetSightingsByUserId(invalidUserId);

            // Assert
            Assert.That(result, Is.InstanceOf<BadRequestObjectResult>());
        }

        [Test]
        public async Task GetSightings_WhenServiceThrowsException_ReturnsInternalServerError()
        {
            // Arrange
            _mockSightingService
                .Setup(s => s.GetSightingsAsync())
                .ThrowsAsync(new Exception("Database connection failed"));

            // Act
            var result = await _controller.GetSightings();

            // Assert
            Assert.That(result, Is.InstanceOf<ObjectResult>());
            var objectResult = result as ObjectResult;
            Assert.That(objectResult.StatusCode, Is.EqualTo(500));
        }
    }
}
