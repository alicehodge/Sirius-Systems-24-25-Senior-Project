using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using StorkDorkMain.Models;
using StorkDork.Areas.Identity.Pages.Account;


namespace StorkDorkMain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly StorkDorkDbContext _context;

    public UserController(UserManager<IdentityUser> userManager, StorkDorkDbContext context)
    {
        _userManager = userManager;
        _context = context;
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
}
