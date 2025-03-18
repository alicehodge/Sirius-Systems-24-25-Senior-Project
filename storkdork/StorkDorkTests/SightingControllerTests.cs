using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Controllers;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using System.Threading.Tasks;
using StorkDorkMain.Models.DTO;

namespace StorkDorkTests;

[TestFixture]
public class SightingControllerTests
{
    private Mock<ISightingService> _sightingService;
    private SightingController _sightingController;

    [SetUp]
    public void setup()
    {
        _sightingService = new Mock<ISightingService>();
        _sightingController = new SightingController(_sightingService.Object);
    }

    [Test]
    public async Task UpdateSightingLocation_ValidRequest_ReturnsOk()
    {
        // Arrange
        var update = new LocationUpdate { SightingId = 1, Country = "USA", Subdivision = "Oregon" };

        // Act
        var result = await _sightingController.UpdateSightingLocation(update);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.AreEqual(200, okResult.StatusCode);
        Assert.AreEqual("Location updates successfully", okResult.Value);
    }

        [Test]
    public async Task UpdateSightingLocation_InvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var update = new LocationUpdate { SightingId = 0, Country = "USA", Subdivision = "Oregon" };

        // Act
        var result = await _sightingController.UpdateSightingLocation(update);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.NotNull(badRequestResult);
        Assert.AreEqual(400, badRequestResult.StatusCode);
        Assert.AreEqual("Invalid request data.", badRequestResult.Value);
    }

    [Test]
    public async Task UpdateSightingLocation_SightingNotFound_ReturnsNotFound()
    {
        // Arrange
        var update = new LocationUpdate { SightingId = 1, Country = "USA", Subdivision = "Oregon" };
        _sightingService.Setup(s => s.UpdateSightingLocationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                            .Throws(new KeyNotFoundException());

        // Act
        var result = await _sightingController.UpdateSightingLocation(update);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        Assert.NotNull(notFoundResult);
        Assert.AreEqual(404, notFoundResult.StatusCode);
        Assert.AreEqual("Sighting not found", notFoundResult.Value);
    }

    [Test]
    public async Task UpdateSightingLocation_UnexpectedError_ReturnsServerError()
    {
        // Arrange
        var update = new LocationUpdate { SightingId = 1, Country = "USA", Subdivision = "Oregon" };
        _sightingService.Setup(s => s.UpdateSightingLocationAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                            .Throws(new Exception("Something went wrong"));

        // Act
        var result = await _sightingController.UpdateSightingLocation(update);

        // Assert
        var serverErrorResult = result as ObjectResult;
        Assert.NotNull(serverErrorResult);
        Assert.AreEqual(500, serverErrorResult.StatusCode);
        Assert.AreEqual("Internal Server Error: Something went wrong", serverErrorResult.Value);
    }
}