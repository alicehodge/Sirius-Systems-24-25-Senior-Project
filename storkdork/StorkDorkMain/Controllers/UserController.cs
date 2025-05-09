using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using StorkDorkMain.Models;
using StorkDorkMain.Data;
using StorkDork.Areas.Identity.Pages.Account;
using StorkDorkMain.DAL.Abstract;
using NuGet.Protocol;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium.Internal;

namespace StorkDorkMain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly StorkDorkDbContext _context;
    private readonly ISDUserRepository _sdUserRepository;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserController(
        UserManager<IdentityUser> userManager, 
        StorkDorkDbContext context, 
        ISDUserRepository sdUserRepository, 
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _sdUserRepository = sdUserRepository ?? throw new ArgumentNullException(nameof(sdUserRepository));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateUser([FromBody] RegisterModel.InputModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = new IdentityUser { UserName = model.Email, Email = model.Email };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            var sdUser = new SdUser
            {
                AspNetIdentityId = user.Id // Store the AspNetIdentityId
            };

            _context.SdUsers.Add(sdUser);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User and associated SdUser created successfully" });
        }

        return BadRequest(result.Errors);
    }

    [HttpGet("is-user-logged-in")]
    public bool isUserLoggedIn()
    {
        return User?.Identity?.IsAuthenticated ?? false;
    }

    // Gets the sdUser for use in javascript
    [HttpGet("current-user")]
    public async Task<IActionResult> GetUserId()
    {
        try
        {
            if (User == null || !User.Identity.IsAuthenticated)
                return Unauthorized("User is not authenticated.");

            var sdUser = await _sdUserRepository.GetSDUserByIdentity(User);

            if (sdUser == null)
                return NotFound("User not found.");

            return Ok(sdUser);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("assign-role")]
    public async Task<IActionResult> AssignRole(string userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("User not found");
        }

        if (!await _roleManager.RoleExistsAsync(roleName))
        {
            return BadRequest("Role does not exist");
        }

        var result = await _userManager.AddToRoleAsync(user, roleName);
        if (result.Succeeded)
        {
            return Ok($"User assigned to role {roleName} successfully");
        }

        return BadRequest(result.Errors);
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("users-with-roles")]
    public async Task<IActionResult> GetUsersWithRoles()
    {
        var users = await _userManager.Users.ToListAsync();
        var userRoles = new List<object>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userRoles.Add(new
            {
                UserId = user.Id,
                Email = user.Email,
                Roles = roles
            });
        }

        return Ok(userRoles);
    }
}
