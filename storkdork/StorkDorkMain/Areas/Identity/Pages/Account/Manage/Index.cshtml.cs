using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StorkDorkMain.Models;
using StorkDorkMain.DAL.Abstract;
using System.Threading.Tasks;

namespace StorkDorkMain.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISDUserRepository _sdUserRepository;
        private readonly IMilestoneRepository _milestoneRepository;

        public IndexModel(UserManager<IdentityUser> userManager, ISDUserRepository sdUserRepository, IMilestoneRepository milestoneRepository)
        {
            _userManager = userManager;
            _sdUserRepository = sdUserRepository;
            _milestoneRepository = milestoneRepository;
        }

        public string Email { get; set; }

        public int SightingsMade { get; set; }
        public int PhotosContributed { get; set; }
        public string MilestoneTier { get; set; }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string DisplayName { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Email = user.Email;

            // Load your custom StorkDork user info
            var sdUser = await _sdUserRepository.GetSDUserByIdentity(User);

            if (sdUser != null)
            {
                Input = new InputModel
                {
                    DisplayName = sdUser.DisplayName
                };

                // Get Sightings and Photos
                SightingsMade = await _milestoneRepository.GetSightingsMade(sdUser.Id);
                PhotosContributed = await _milestoneRepository.GetPhotosContributed(sdUser.Id);

                // Milestone Tier based on achievements
                int tier = _milestoneRepository.GetMilestoneTier(SightingsMade);
                MilestoneTier = GetTierName(tier);
            }

            return Page();
        }

        private string GetTierName(int tier)
        {
            return tier switch
            {
                1 => "Gold",
                2 => "Silver",
                3 => "Bronze",
                _ => "No Tier"
            };
        }
    }
}
