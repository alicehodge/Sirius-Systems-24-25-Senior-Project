using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using Reqnroll;
using System.Linq;
using StorkDorkMain.Controllers;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models.DTO;
using StorkDorkMain.Services;

[Binding]
public class GetSightingsByUserSteps
{
    private readonly Mock<ISightingService> _mockSightingService = new();
    private readonly Mock<ISDUserRepository> _MockUserRepo = new();
    private readonly Mock<IEBirdService> _MockEBirdService = new();
    private MapApiController _controller;

    private int _userId;
    private IActionResult _response;
    private List<SightMarker> _expectedSightings;

    [Given(@"a valid user ID of (.*)")]
    public void GivenAValidUserIdOf(int userId)
    {
        _userId = userId;
        _controller = new MapApiController(_mockSightingService.Object, _MockUserRepo.Object, _MockEBirdService.Object);
    }

    [Given(@"the user had (.*) sightings")]
    public void GivenTheUserHadSightings(int count)
    {
        _expectedSightings = new List<SightMarker>();
        for (int i = 0; i < count; i++)
        {
            _expectedSightings.Add(new SightMarker
            {
                SightingId = 1 + i,
                CommonName = $"Red-wing Blackbird ({i + 1})",
                SciName = "Agelaius phoeniceus",
                Longitude = (decimal?)44.78 + i,
                Latitude = (decimal?)-123.07 - i,
                Description = "A common bird in North America",
                Date = DateTime.Now,
                Country = "USA",
                Subdivision = "Oregon",
                Birder = "Emily Carter"
            });
        }

        _mockSightingService
            .Setup(s => s.GetSightingsByUserIdAsync(_userId))
            .ReturnsAsync(_expectedSightings);
    }

    [When(@"I request the sightings for the user")]
    public async Task WhenIRequestTheSightingsForTheUser()
    {
        Assert.That(_controller, Is.Not.Null, "Controller was not initialized");
        _response = await _controller.GetSightingsByUser(_userId);
    }

    [Then(@"the response should be OK")]
    public void ThenTheResponseShouldBeOK()
    {
        Assert.That(_response, Is.InstanceOf<OkObjectResult>());
    }

    [Then(@"the result should contain (.*) sightings")]
    public void ThenTheResultShouldContainSightings(int expectedCount)
    {
        var result = (_response as OkObjectResult)?.Value as IEnumerable<SightMarker>;
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Count(), Is.EqualTo(expectedCount));
    }

    [Given(@"an invalid user ID of (.*)")]
    public void GivenAnInvalidUserIdOf(int userId)
    {
        _userId = userId;

        _controller = new MapApiController(
            _mockSightingService.Object,
            _MockUserRepo.Object,
            _MockEBirdService.Object
        );
    }

    [Then(@"the response should be a BadRequest")]
    public void ThenTheResponseShouldBeABadRequest()
    {
        Assert.That(_response, Is.InstanceOf<BadRequestObjectResult>());
    }

    [AfterScenario]
    public void Cleanup()
    {
        _response = null;
        _expectedSightings = null;
        _userId = 0;

        // Dispose mocks if needed (not usually necessary for Moq, but safe to include)
        _mockSightingService.Reset();
        _MockUserRepo.Reset();
        _MockEBirdService.Reset();
        
        // Force GC if you're suspicious:
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}