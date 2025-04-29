using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace StorkDorkMain.Services;

public class RoleInitializerService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<RoleInitializerService> _logger;

    public RoleInitializerService(
        RoleManager<IdentityRole> roleManager,
        UserManager<IdentityUser> userManager,
        ILogger<RoleInitializerService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task InitializeRoles()
    {
        try
        {
            var roles = new[] { "Admin", "Moderator", "User" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    await _roleManager.CreateAsync(new IdentityRole(role));
                    _logger.LogInformation($"Created role: {role}");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing roles");
            throw;
        }
    }

    public async Task CreateAdminUser(string email, string password)
    {
        try
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
                    _logger.LogInformation($"Created admin user: {email}");
                }
                else
                {
                    _logger.LogError($"Failed to create admin user: {string.Join(", ", result.Errors)}");
                }
            }
            else if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
                _logger.LogInformation($"Added existing user to Admin role: {email}");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating admin user");
            throw;
        }
    }

    public async Task AssignAdminRole(string email)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                _logger.LogError($"Failed to assign admin role: User not found with email {email}");
                return;
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var result = await _userManager.AddToRoleAsync(user, "Admin");
                if (result.Succeeded)
                {
                    _logger.LogInformation($"Successfully assigned Admin role to user: {email}");
                }
                else
                {
                    _logger.LogError($"Failed to assign Admin role: {string.Join(", ", result.Errors)}");
                }
            }
            else
            {
                _logger.LogInformation($"User {email} is already an Admin");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error assigning Admin role to {email}");
            throw;
        }
    }
}