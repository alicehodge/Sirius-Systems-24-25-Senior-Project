using Microsoft.AspNetCore.Identity;

namespace StorkDorkMain.Services;

public class RoleInitializerService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;

    public RoleInitializerService(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task InitializeRoles()
    {
        string[] roles = { "Admin", "Moderator", "User" };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    public async Task CreateAdminUser(string email, string password)
    {
        var adminUser = await _userManager.FindByEmailAsync(email);

        if (adminUser == null)
        {
            adminUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(adminUser, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}