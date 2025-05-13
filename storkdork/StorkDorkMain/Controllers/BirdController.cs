using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Models;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

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

            var bird = await _birdRepo.GetAll()
                .Include(b => b.Photos)
                .FirstOrDefaultAsync(b => b.Id == id.Value);

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

                // Check authentication first
                var submitter = await _sdUserRepository.GetSDUserByIdentity(User);
                if (submitter == null)
                {
                    _logger.LogWarning("Unauthorized access attempt - no valid user found");
                    return new UnauthorizedResult();
                }

                ModelState.Remove("SubmissionNotes");

                if (!ModelState.IsValid)
                {
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

                    TempData["SuccessMessage"] = "Your range information has been submitted for review. Thank you for contributing!";
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

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitPhoto(BirdPhotoSubmissionViewModel viewModel)
        {
            try
            {
                _logger.LogInformation($"Processing photo submission for BirdId: {viewModel.BirdId}");

                // Check authentication first
                var submitter = await _sdUserRepository.GetSDUserByIdentity(User);
                if (submitter == null)
                {
                    _logger.LogWarning("Unauthorized access attempt - no valid user found");
                    return new UnauthorizedResult();
                }

                ModelState.Remove("PhotoData");
                ModelState.Remove("PhotoContentType");

                if (!ModelState.IsValid)
                {
                    // Return any errors to the view
                    foreach (var state in ModelState)
                    {
                        foreach (var error in state.Value.Errors)
                        {
                            _logger.LogWarning($"ModelState error for {state.Key}: {error.ErrorMessage}");
                        }
                    }
                    return await ReloadDetailsView(viewModel.BirdId, "Please check the form values.");
                }

                // Find the respective Bird
                var existingBird = _birdRepo.FindById(viewModel.BirdId);
                if (existingBird == null)
                {
                    return NotFound($"Bird with ID {viewModel.BirdId} not found");
                }

                // Ensure photo is provided
                if (viewModel.Photo == null || viewModel.Photo.Length == 0)
                {
                    ModelState.AddModelError("Photo", "A photo is required");
                    return await ReloadDetailsView(viewModel.BirdId, "Please upload a photo.");
                }

                // Convert the IFormFile (viewModel.Photo) into byte[] and content type
                byte[] photoBytes;
                string photoContentType;

                if (viewModel.Photo.Length > 5 * 1024 * 1024) // limit size to 5MB
                {
                    ModelState.AddModelError("Photo", "File size exceeds 5MB limit");
                    return await ReloadDetailsView(viewModel.BirdId, "File size exceeds 5MB limit");
                }

                if (!viewModel.Photo.ContentType.StartsWith("image/"))
                {
                    ModelState.AddModelError("Photo", "Only image files are allowed");
                    return await ReloadDetailsView(viewModel.BirdId, "Invalid file type");
                }

                using var memoryStream = new MemoryStream();
                await viewModel.Photo.CopyToAsync(memoryStream);
                photoBytes = memoryStream.ToArray();
                photoContentType = viewModel.Photo.ContentType;

                // Create moderated content from view model
                var submission = new BirdPhotoSubmission
                {
                    BirdId = viewModel.BirdId,
                    Bird = _birdRepo.FindById(viewModel.BirdId),
                    PhotoData = photoBytes,
                    PhotoContentType = photoContentType,
                    Caption = viewModel.Caption,
                    ContentType = "BirdPhoto",
                    Status = "Pending",
                    SubmittedDate = DateTime.UtcNow,
                    SubmitterId = submitter.Id
                };

                try
                {
                    _logger.LogInformation($"Saving photo submission with SubmitterId: {submitter.Id}");
                    _moderatedContentRepository.AddOrUpdate(submission);
                    await _moderatedContentRepository.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Your photo has been submitted for review. Thank you for contributing!";
                    return RedirectToAction(nameof(Details), new { id = viewModel.BirdId });
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error saving photo submission: {ex.Message}");
                    return await ReloadDetailsView(viewModel.BirdId, "Unable to save your submission.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in SubmitPhoto: {ex.Message}, Stack Trace: {ex.StackTrace}");
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