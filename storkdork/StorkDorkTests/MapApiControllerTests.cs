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
    }
}
