using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Models;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace StorkDork.Controllers
{
    public class BirdController : Controller
    {
        private readonly IBirdRepository _birdRepo;
        private readonly IModeratedContentRepository _moderatedContentRepository;
        private readonly ILogger<BirdController> _logger;
        private readonly ISDUserRepository _sdUserRepository;

        public BirdController(IBirdRepository birdRepo, IModeratedContentRepository moderatedContentRepository, ISDUserRepository sDUserRepository, ILogger<BirdController> logger)
        {
            _birdRepo = birdRepo;
            _moderatedContentRepository = moderatedContentRepository;
            _sdUserRepository = sDUserRepository;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string? categoryFilter = null, int page = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bird = _birdRepo.FindById(id.Value);
            if (bird == null)
            {
                return NotFound();
            }

            var (relatedBirds, totalCount) = await _birdRepo.GetRelatedBirds(
                bird.SpeciesCode, 
                bird.ReportAs,
                categoryFilter,
                page);

            var viewModel = new BirdDetailsViewModel
            {
                Bird = bird,
                RelatedBirds = relatedBirds.Select(rb => new RelatedBirdViewModel 
                {
                    Bird = rb,
                    RelationType = rb.Category
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / 10.0),
                CategoryFilter = categoryFilter
            };

            return View(viewModel);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRange(RangeSubmissionViewModel viewModel)
        {
            try
            {
                _logger.LogInformation($"Processing range submission for BirdId: {viewModel.BirdId}");

                if (!ModelState.IsValid)
                {
                    // Log validation errors
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning($"ModelState error for {state.Key}: {error.ErrorMessage}");
                        }
                    }
                    
                    return await ReloadDetailsView(viewModel.BirdId, "Please check the form values.");
                }

                var existingBird = _birdRepo.FindById(viewModel.BirdId);
                if (existingBird == null)
                {
                    return NotFound($"Bird with ID {viewModel.BirdId} not found");
                }

                var submitter = await _sdUserRepository.GetSDUserByIdentity(User);
                if (submitter == null)
                {
                    _logger.LogError("Failed to retrieve SdUser for the current user");
                    return await ReloadDetailsView(viewModel.BirdId, "User account error. Please try again later.");
                }

                // Create moderated content from view model
                var submission = new RangeSubmission
                {
                    BirdId = viewModel.BirdId,
                    Bird = existingBird,
                    RangeDescription = viewModel.RangeDescription,
                    SubmissionNotes = viewModel.SubmissionNotes,
                    ContentType = "BirdRange",
                    Status = "Pending",
                    SubmittedDate = DateTime.UtcNow,
                    SubmitterId = submitter.Id
                };

                try
                {
                    _logger.LogInformation($"Saving range submission with SubmitterId: {submitter.Id}");
                    _moderatedContentRepository.AddOrUpdate(submission);
                    await _moderatedContentRepository.SaveChangesAsync();

                    TempData["Message"] = "Your range information has been submitted for review.";
                    return RedirectToAction(nameof(Details), new { id = viewModel.BirdId });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving range submission: {ex.Message}");
                    return await ReloadDetailsView(viewModel.BirdId, "Unable to save your submission.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SubmitRange: {ex.Message}, Stack Trace: {ex.StackTrace}");
                return RedirectToAction("Error", "Home");
            }
        }

        private async Task<IActionResult> ReloadDetailsView(int birdId, string errorMessage)
        {
            try
            {
                var bird = _birdRepo.FindById(birdId);
                if (bird == null)
                {
                    return NotFound($"Bird with ID {birdId} not found");
                }

                var (relatedBirds, totalCount) = await _birdRepo.GetRelatedBirds(
                    bird.SpeciesCode,
                    bird.ReportAs,
                    null,
                    1
                );

                var viewModel = new BirdDetailsViewModel
                {
                    Bird = bird,
                    RelatedBirds = relatedBirds.Select(rb => new RelatedBirdViewModel
                    {
                        Bird = rb,
                        RelationType = rb.Category
                    }).ToList(),
                    CurrentPage = 1,
                    TotalPages = (int)Math.Ceiling(totalCount / 10.0)
                };

                ModelState.AddModelError("", errorMessage);
                return View("Details", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error reloading details view: {ex.Message}");
                throw;
            }
        }
    }
}