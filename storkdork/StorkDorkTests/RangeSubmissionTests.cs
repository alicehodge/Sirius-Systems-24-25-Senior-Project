using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using StorkDork.Controllers;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace StorkDorkTests
{
    [TestFixture]
    public class RangeSubmissionTests : IDisposable
    {
        private Mock<IBirdRepository> _mockBirdRepo;
        private Mock<IModeratedContentRepository> _mockModeratedRepo;
        private Mock<ISDUserRepository> _mockUserRepo;
        private Mock<ILogger<BirdController>> _mockLogger;
        private BirdController _controller;

        [SetUp]
        public void Setup()
        {
            _mockBirdRepo = new Mock<IBirdRepository>();
            _mockModeratedRepo = new Mock<IModeratedContentRepository>();
            _mockUserRepo = new Mock<ISDUserRepository>();
            _mockLogger = new Mock<ILogger<BirdController>>();

            _controller = new BirdController(
                _mockBirdRepo.Object,
                _mockModeratedRepo.Object,
                _mockUserRepo.Object,
                _mockLogger.Object
            );

            // Setup common test data
            var testBird = new Bird { Id = 1, CommonName = "Great Blue Heron" };
            var testUser = new SdUser { Id = 1 };

            _mockBirdRepo.Setup(r => r.FindById(1)).Returns(testBird);
            _mockUserRepo.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(testUser);
        }

        [Test]
        public async Task SubmitRange_WithEmptyDescription_ReturnsValidationError()
        {
            // Arrange
            var viewModel = new RangeSubmissionViewModel
            {
                BirdId = 1,
                RangeDescription = "",
                SubmissionNotes = "Test notes"
            };
            _controller.ModelState.AddModelError("RangeDescription", "Please provide range information");

            // Act
            var result = await _controller.SubmitRange(viewModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<ViewResult>());
                var viewResult = (ViewResult)result;
                Assert.That(viewResult.ViewName, Is.EqualTo("Details"));
                Assert.That(viewResult.ViewData.ModelState.IsValid, Is.False);
                Assert.That(viewResult.ViewData.ModelState["RangeDescription"].Errors[0].ErrorMessage, 
                    Is.EqualTo("Please provide range information"));
            });
        }

        [Test]
        public async Task SubmitRange_WithShortDescription_ReturnsValidationError()
        {
            // Arrange
            var viewModel = new RangeSubmissionViewModel
            {
                BirdId = 1,
                RangeDescription = "USA",
                SubmissionNotes = "Test notes"
            };
            _controller.ModelState.AddModelError("RangeDescription", 
                "Range description should be at least 5 characters");

            // Act
            var result = await _controller.SubmitRange(viewModel);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result, Is.InstanceOf<ViewResult>());
                var viewResult = (ViewResult)result;
                Assert.That(viewResult.ViewData.ModelState.IsValid, Is.False);
                Assert.That(viewResult.ViewData.ModelState["RangeDescription"].Errors[0].ErrorMessage,
                    Is.EqualTo("Range description should be at least 5 characters"));
            });
        }

        [Test]
        public async Task SubmitRange_WithoutAuthentication_ReturnsUnauthorized()
        {
            // Arrange
            var viewModel = new RangeSubmissionViewModel
            {
                BirdId = 1,
                RangeDescription = "Test Range",
                SubmissionNotes = "Test Notes"
            };
            _mockUserRepo.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync((SdUser)null);

            // Act
            var result = await _controller.SubmitRange(viewModel);

            // Assert
            Assert.That(result, Is.InstanceOf<UnauthorizedResult>());
        }

        // [Test]
        // public async Task SubmitRange_WithValidData_SuccessfullySubmits()
        // {
        //     // Arrange
        //     var testBird = new Bird 
        //     { 
        //         Id = 1, 
        //         CommonName = "Great Blue Heron",
        //         SpeciesCode = "grbher",
        //         Range = "Current range information"
        //     };

        //     var testUser = new SdUser 
        //     { 
        //         Id = 1,
        //         FirstName = "Test",
        //         LastName = "User"
        //     };

        //     _mockBirdRepo.Setup(r => r.FindById(1)).Returns(testBird);
        //     _mockUserRepo.Setup(r => r.GetSDUserByIdentity(It.IsAny<ClaimsPrincipal>()))
        //         .ReturnsAsync(testUser);

        //     var viewModel = new RangeSubmissionViewModel
        //     {
        //         BirdId = 1,
        //         RangeDescription = "Found throughout North America, breeding from southern Canada through Florida.",
        //         SubmissionNotes = "Based on recent observations"
        //     };

        //     RangeSubmission capturedSubmission = null;
        //     _mockModeratedRepo
        //         .Setup(r => r.AddOrUpdate(It.IsAny<ModeratedContent>()))
        //         .Callback<ModeratedContent>(content => 
        //         {
        //             capturedSubmission = (RangeSubmission)content;
        //             // Simulate the repository setting the ID
        //             capturedSubmission.Id = 1;
        //             capturedSubmission.ContentId = 1;
        //         });

        //     // Set up model state
        //     _controller.ModelState.Clear();
            
        //     // Set up controller context
        //     var controllerContext = new ControllerContext
        //     {
        //         HttpContext = new DefaultHttpContext
        //         {
        //             User = CreateMockUser(testUser.Id.ToString())
        //         }
        //     };
        //     _controller.ControllerContext = controllerContext;

        //     // Act
        //     var result = await _controller.SubmitRange(viewModel);

        //     // Assert
        //     Assert.Multiple(() =>
        //     {
        //         // Verify the result type and redirect
        //         Assert.That(result, Is.InstanceOf<RedirectToActionResult>());
        //         var redirectResult = (RedirectToActionResult)result;
        //         Assert.That(redirectResult.ActionName, Is.EqualTo("Details"));
        //         Assert.That(redirectResult.RouteValues["id"], Is.EqualTo(1));

        //         // Verify the captured submission
        //         Assert.That(capturedSubmission, Is.Not.Null, "Submission should not be null");
        //         Assert.That(capturedSubmission, Is.InstanceOf<RangeSubmission>(), "Should be a RangeSubmission");
                
        //         // Verify ModeratedContent base properties
        //         Assert.That(capturedSubmission.ContentType, Is.EqualTo("BirdRange"));
        //         Assert.That(capturedSubmission.Status, Is.EqualTo("Pending"));
        //         Assert.That(capturedSubmission.SubmitterId, Is.EqualTo(testUser.Id));
        //         Assert.That(capturedSubmission.SubmittedDate, Is.Not.EqualTo(default(DateTime)));
        //         Assert.That(capturedSubmission.ModeratorId, Is.Null);
        //         Assert.That(capturedSubmission.ModeratedDate, Is.Null);
                
        //         // Verify RangeSubmission specific properties
        //         Assert.That(capturedSubmission.BirdId, Is.EqualTo(viewModel.BirdId));
        //         Assert.That(capturedSubmission.RangeDescription, Is.EqualTo(viewModel.RangeDescription));
        //         Assert.That(capturedSubmission.SubmissionNotes, Is.EqualTo(viewModel.SubmissionNotes));
        //         Assert.That(capturedSubmission.Bird, Is.SameAs(testBird));

        //         // Verify repository calls
        //         _mockModeratedRepo.Verify(r => r.AddOrUpdate(It.IsAny<RangeSubmission>()), Times.Once);
        //         _mockModeratedRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        //     });
        // }

        private static ClaimsPrincipal CreateMockUser(string userId)
        {
            var claims = new[] { new Claim(ClaimTypes.NameIdentifier, userId) };
            var identity = new ClaimsIdentity(claims, "Test");
            return new ClaimsPrincipal(identity);
        }

        [TearDown]
        public void TearDown()
        {
            Dispose();
        }

        public void Dispose()
        {
            _mockBirdRepo.Reset();
            _mockModeratedRepo.Reset();
            _mockUserRepo.Reset(); 
            _mockLogger.Reset();
            _controller?.Dispose();
            _controller = null;
        }
    }
}