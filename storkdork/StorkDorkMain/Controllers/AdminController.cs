using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.Models;

namespace StorkDorkMain.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> UserRoles()
        {
            var users = await _userManager.Users.ToListAsync();
            var viewModel = new UserRoleManagementViewModel
            {
                Users = await GetUserRoleViewModels(users),
                AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync(),
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.LockoutEnd == null || u.LockoutEnd < DateTime.UtcNow),
                RoleCounts = await GetRoleCounts()
            };

            return View(viewModel);
        }

        private async Task<IEnumerable<UserRoleViewModel>> GetUserRoleViewModels(List<IdentityUser> users)
        {
            var userViewModels = new List<UserRoleViewModel>();
            
            foreach (var user in users)
            {
                userViewModels.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    Roles = await _userManager.GetRolesAsync(user),
                    LastLogin = user.LockoutEnd?.DateTime,
                    IsActive = user.LockoutEnd == null || user.LockoutEnd < DateTime.UtcNow
                });
            }

            return userViewModels;
        }

        private async Task<Dictionary<string, int>> GetRoleCounts()
        {
            var roleCounts = new Dictionary<string, int>();
            var roles = await _roleManager.Roles.ToListAsync();

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
                roleCounts[role.Name] = usersInRole.Count;
            }

            return roleCounts;
        }
    }
}