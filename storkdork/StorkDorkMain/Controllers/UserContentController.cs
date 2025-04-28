using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDork.Controllers
{
    [Authorize]
    public class UserContentController : Controller
    {
        private readonly IModeratedContentRepository _moderatedContentRepository;
        private readonly ISDUserRepository _sdUserRepository;
        private readonly ILogger<UserContentController> _logger;

        public UserContentController(
            IModeratedContentRepository moderatedContentRepository,
            ISDUserRepository sdUserRepository,
            ILogger<UserContentController> logger)
        {
            _moderatedContentRepository = moderatedContentRepository;
            _sdUserRepository = sdUserRepository;
            _logger = logger;
        }

        public async Task<IActionResult> MySubmissions()
        {
            try
            {
                var user = await _sdUserRepository.GetSDUserByIdentity(User);
                if (user == null)
                {
                    return Challenge();
                }

                var submissions = _moderatedContentRepository.GetAll()
                    .Where(c => c.SubmitterId == user.Id)
                    .OrderByDescending(c => c.SubmittedDate);

                return View(submissions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user submissions");
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<IActionResult> SubmissionDetails(int id)
        {
            try
            {
                var user = await _sdUserRepository.GetSDUserByIdentity(User);
                if (user == null)
                {
                    return Challenge();
                }
        
                var submission = await _moderatedContentRepository.GetContentWithDetailsAsync(id);
                if (submission == null || submission.SubmitterId != user.Id)
                {
                    return NotFound();
                }
        
                return View(submission);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving submission details for ID: {id}");
                return RedirectToAction("Error", "Home");
            }
        }
    }
}