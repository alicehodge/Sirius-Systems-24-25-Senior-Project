using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Models;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Services;
using Microsoft.Extensions.Logging;

namespace StorkDorkMain.Controllers
{
    [Authorize(Roles = "Admin,Moderator")]
    public class ModerationController : Controller
    {
        private readonly IModerationService _moderationService;
        private readonly UserManager<IdentityUser> _userManager;
        private IBirdRepository _birdRepository;
        private readonly ILogger<ModerationController> _logger;

        public ModerationController(IModerationService moderationService, UserManager<IdentityUser> userManager, IBirdRepository birdRepository, ILogger<ModerationController> logger)
        {
            _moderationService = moderationService;
            _userManager = userManager;
            _birdRepository = birdRepository;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var pendingContent = await _moderationService.GetPendingContentAsync();
            return View(pendingContent);
        }

        public async Task<IActionResult> Details(int id)
        {
            var content = await _moderationService.GetContentDetailsAsync(id);
            if (content == null) return NotFound();
            
            // Log the actual type and content type for debugging
            _logger.LogInformation($"Content type: {content.ContentType}, Actual type: {content.GetType().Name}");
            
            // Handle different content types appropriately
            if (content.ContentType == "BirdRange" && content is RangeSubmission)
            {
                var rangeSubmission = content as RangeSubmission;
                
                // If Bird navigation property is null, load it
                if (rangeSubmission.Bird == null)
                {
                    rangeSubmission.Bird = _birdRepository.FindById(rangeSubmission.BirdId);
                }
                
                return View("Details", rangeSubmission);
            }
            
            return View(content);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveContent(int id, string notes)
        {
            var content = await _moderationService.GetContentDetailsAsync(id);
            if (content == null)
            {
                return NotFound();
            }

            bool success;
            if (content is RangeSubmission)
            {
                success = await _moderationService.ApproveRangeSubmission(id, User, notes);
                if (success)
                {
                    TempData["SuccessMessage"] = "Range information has been approved successfully";
                }
            }
            else
            {
                success = await _moderationService.ApproveContentAsync(id, User, notes);
                TempData["SuccessMessage"] = "Content has been approved successfully";
            }

            if (success)
            {
                return RedirectToAction(nameof(Index));
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectContent(int id, string notes)
        {
            try
            {
                _logger.LogInformation($"Attempting to reject content with ID {id}");

                if (string.IsNullOrEmpty(notes))
                {
                    _logger.LogWarning($"Rejection attempted without notes for content ID {id}");
                    return BadRequest("Rejection reason is required");
                }

                var content = await _moderationService.GetContentDetailsAsync(id);
                if (content == null)
                {
                    _logger.LogWarning($"Content with ID {id} not found during rejection attempt");
                    return NotFound($"Content with ID {id} not found");
                }

                _logger.LogInformation($"Processing rejection for content ID {id}, type: {content.ContentType}");
                var success = await _moderationService.RejectContentAsync(id, User, notes);
                
                if (success)
                {
                    _logger.LogInformation($"Successfully rejected content ID {id}");
                    TempData["SuccessMessage"] = "Content has been rejected successfully";
                    return RedirectToAction(nameof(Index));
                }

                _logger.LogError($"Failed to reject content ID {id}");
                return BadRequest("Failed to reject content");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error rejecting content ID {id}");
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        public async Task<IActionResult> History()
        {
            var history = await _moderationService.GetModerationHistoryAsync();
            return View(history);
        }

        [HttpGet]
        public async Task<IActionResult> FilterByStatus(string status)
        {
            var content = await _moderationService.GetContentByStatusAsync(status);
            return PartialView("_ContentList", content);
        }
    }
}