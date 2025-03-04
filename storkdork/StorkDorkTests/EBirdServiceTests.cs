// StorkDorkTests/EBirdServiceTests.cs
using NUnit.Framework;
using Moq;
using Moq.Protected;
using StorkDorkMain.Services;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Http;

namespace StorkDorkTests
{
    [TestFixture]
    public class EBirdServiceTests
    {
        private Mock<HttpMessageHandler> _mockHttpHandler;
        private HttpClient _httpClient;
        private Mock<IConfiguration> _mockConfig;
        private Mock<IBirdRepository> _mockBirdRepo;
        private EBirdService _eBirdService;

        [SetUp]
        public void Setup()
        {
            _mockHttpHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpHandler.Object);
            _mockConfig = new Mock<IConfiguration>();
            _mockBirdRepo = new Mock<IBirdRepository>();

            _mockConfig.Setup(c => c["EBird:ApiKey"]).Returns("test-api-key");
            _eBirdService = new EBirdService(_httpClient, _mockConfig.Object, _mockBirdRepo.Object);
        }

        [Test]
        public async Task GetNearestSightings_ValidBird_ReturnsSightings()
        {
            // Arrange
            var birdId = 1;
            var bird = new Bird
            {
                Id = birdId,
                CommonName = "Canada Goose",
                SpeciesCode = "cangoo"
            };
            var jsonResponse = @"[
                {
                    ""speciesCode"": ""cangoo"",
                    ""comName"": ""Canada Goose"",
                    ""locName"": ""Test Location"",
                    ""obsDt"": ""2024-03-06 10:00"",
                    ""howMany"": 2,
                    ""lat"": 44.8485,
                    ""lng"": -123.2340
                }
            ]";

            _mockBirdRepo.Setup(r => r.FindById(birdId)).Returns(bird);
            SetupMockHttpResponse(jsonResponse);

            // Act
            var sightings = await _eBirdService.GetNearestSightings(birdId, 44.8485, -123.2340);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(sightings, Is.Not.Empty);
                var sighting = sightings.First();
                Assert.That(sighting.BirdId, Is.EqualTo(birdId));
                Assert.That(sighting.Latitude, Is.EqualTo(44.8485m));
                Assert.That(sighting.Longitude, Is.EqualTo(-123.2340m));
            });
        }

        [Test]
        public async Task GetNearestSightings_InvalidBird_ReturnsEmptyList()
        {
            // Arrange
            var birdId = -1;
            _mockBirdRepo.Setup(r => r.FindById(birdId)).Returns((Bird)null);

            // Act
            var sightings = await _eBirdService.GetNearestSightings(birdId, 44.8485, -123.2340);

            // Assert
            Assert.That(sightings, Is.Empty);
        }

        [Test]
        public async Task GetNearestSightings_ApiError_ReturnsEmptyList()
        {
            // Arrange
            var birdId = 1;
            var bird = new Bird { Id = birdId, SpeciesCode = "cangoo" };
            _mockBirdRepo.Setup(r => r.FindById(birdId)).Returns(bird);
            SetupMockHttpResponse("", HttpStatusCode.InternalServerError);

            // Act
            var sightings = await _eBirdService.GetNearestSightings(birdId, 44.8485, -123.2340);

            // Assert
            Assert.That(sightings, Is.Empty);
        }

        [Test]
        public async Task GetNearestSightings_EmptyResponse_ReturnsEmptyList()
        {
            // Arrange
            var birdId = 1;
            var bird = new Bird { Id = birdId, SpeciesCode = "cangoo" };
            _mockBirdRepo.Setup(r => r.FindById(birdId)).Returns(bird);
            SetupMockHttpResponse("[]");

            // Act
            var sightings = await _eBirdService.GetNearestSightings(birdId, 44.8485, -123.2340);

            // Assert
            Assert.That(sightings, Is.Empty);
        }

        private void SetupMockHttpResponse(string content, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _mockHttpHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(content)
                });
        }

        [TearDown]
        public void TearDown()
        {
            _httpClient.Dispose();
        }
    }
}