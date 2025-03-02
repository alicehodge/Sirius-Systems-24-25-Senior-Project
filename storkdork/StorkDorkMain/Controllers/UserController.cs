using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using StorkDorkMain.Models;
using StorkDork.Areas.Identity.Pages.Account;
using StorkDorkMain.DAL.Abstract;
using NuGet.Protocol;


namespace StorkDorkMain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly StorkDorkDbContext _context;
    private readonly ISDUserRepository _sdUserRepository;

    public UserController(UserManager<IdentityUser> userManager, StorkDorkDbContext context, ISDUserRepository sdUserRepository)
    {
        _userManager = userManager;
        _context = context;
        _sdUserRepository = sdUserRepository;

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
}
