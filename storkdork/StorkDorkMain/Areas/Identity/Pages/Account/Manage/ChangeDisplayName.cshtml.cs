using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace StorkDorkMain.Areas.Identity.Pages.Account.Manage
{
    [Authorize]
    public class ChangeDisplayNameModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ISDUserRepository _sdUserRepository;

        public ChangeDisplayNameModel(UserManager<IdentityUser> userManager, ISDUserRepository sdUserRepository)
        {
            _userManager = userManager;
            _sdUserRepository = sdUserRepository;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(25, ErrorMessage = "Display Name must be at most 25 characters.")]
            [Display(Name = "New Display Name")]
            public string NewDisplayName { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var sdUser = await _sdUserRepository.GetSDUserByIdentity(User);
            if (sdUser == null)
            {
                return NotFound("Unable to find StorkDork user profile.");
            }

            // Update the display name
            sdUser.DisplayName = Input.NewDisplayName;

            // Save changes to the database
            await _sdUserRepository.UpdateAsync(sdUser);

            // Redirect back to profile page
            return RedirectToPage("./Index");
        }
    }
}
