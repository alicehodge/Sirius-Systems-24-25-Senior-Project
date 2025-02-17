using NUnit.Framework;
using Moq;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using StorkDorkMain.Controllers;
using StorkDorkMain.Models;
using StorkDorkMain.Data;

[Test]
public async Task GetSightings_ReturnsCorrectSightings()
{
    // Arrange
    var mockSet = new Mock<DbSet<Sighting>>();
    var sightings = new List<Sighting>
    {
        new Sighting { ID = 1, SDUserID = 1, BirdID = 101, Latitude = 44.85M, Longitude = -123.24M, Notes = "Near WOU" }
    }.AsQueryable();

    mockSet.As<IQueryable<Sighting>>().Setup(m => m.Provider).Returns(sightings.Provider);
    mockSet.As<IQueryable<Sighting>>().Setup(m => m.Expression).Returns(sightings.Expression);
    mockSet.As<IQueryable<Sighting>>().Setup(m => m.ElementType).Returns(sightings.ElementType);
    mockSet.As<IQueryable<Sighting>>().Setup(m => m.GetEnumerator()).Returns(sightings.GetEnumerator());

    var mockContext = new Mock<StorkDorkContext>();
    mockContext.Setup(c => c.Sightings).Returns(mockSet.Object);

    var controller = new SightingsController(mockContext.Object);

    // Act
    var result = await controller.GetSightings();

    // Assert
    Assert.IsNotNull(result);
    Assert.AreEqual(1, result.Count());
    Assert.AreEqual("Near WOU", result.First().Notes);
}
