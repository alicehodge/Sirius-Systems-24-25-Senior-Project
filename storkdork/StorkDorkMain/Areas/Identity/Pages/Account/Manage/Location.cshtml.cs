using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace StorkDork.Areas.Identity.Pages.Account.Manage
{
    public class LocationModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public LocationModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        [TempData]
        public string StatusMessage { get; set; }

        public bool HasLocation { get; set; }
        public string CurrentLocation { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter a valid location")]
            public string Location { get; set; }
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadLocationAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.GetUserAsync(User);
            await UpdateUserLocationAsync(user, Input.Location);
            
            StatusMessage = "Your location has been updated";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRemoveLocationAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            var claims = await _userManager.GetClaimsAsync(user);
            var locationClaims = claims.Where(c => c.Type == "Location");
            
            await _userManager.RemoveClaimsAsync(user, locationClaims);
            await _signInManager.RefreshSignInAsync(user);
            
            StatusMessage = "Your location has been removed";
            return RedirectToPage();
        }

        private async Task LoadLocationAsync(IdentityUser user)
        {
            var locationClaim = (await _userManager.GetClaimsAsync(user))
                .FirstOrDefault(c => c.Type == "Location");
            
            CurrentLocation = locationClaim?.Value;
            HasLocation = locationClaim != null;
            
            Input = new InputModel { Location = CurrentLocation ?? "" };
        }

        private async Task UpdateUserLocationAsync(IdentityUser user, string location)
        {
            var claims = await _userManager.GetClaimsAsync(user);
            var locationClaims = claims.Where(c => c.Type == "Location");
            
            await _userManager.RemoveClaimsAsync(user, locationClaims);
            await _userManager.AddClaimAsync(user, new Claim("Location", location));
            await _signInManager.RefreshSignInAsync(user);
        }
    }
}