using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using StorkDorkMain.Models.DTO;
using System.Linq;

namespace StorkDorkMain.Controllers;

// Handles requests related to the map page
[Route("api/map")]
[ApiController]
public class MapApiController : ControllerBase
{
    private readonly ISightingService _sightingService;
    private readonly ISDUserRepository _sdUserRepository;
    private readonly UserManager<SdUser> _userManager;

    public MapApiController(ISightingService sightingService, ISDUserRepository sdUserRepository, UserManager<SdUser> userManager)
    {
        _sightingService = sightingService;
        _sdUserRepository = sdUserRepository;
        _userManager = userManager;
    }

    // Grabs all sightings in the database
    // Expectations: returns a list of sightings to javascript
    [HttpGet]
    [Route("GetSightings")]
    public async Task<IActionResult> GetSightings()
    {
        try
        {
            var sightings = await _sightingService.GetSightingsAsync();
            return Ok(sightings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Grabs all sightings belonging to a userId
    // Expectations: returns sightings for specified user
    [HttpGet]
    [Route("GetSightings/{userId}/1")]
    public async Task<IActionResult> GetSightingsByUserId(int userId)
    {
        if (userId <= 0)
        {
            return BadRequest("Invalid user ID.");
        }
        
        try
        {
            var sightings = await _sightingService.GetSightingsByUserIdAsync(userId);
            return Ok(sightings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Grabs all sightings belonging to the currently logged in user
    [HttpGet]
    [Route("GetSightings/{_userManager.GetUserId(User)}")]
    public async Task<IActionResult> GetSighitngsByUser()
    {
        var sdUser = await _sdUserRepository.GetSDUserByIdentity(User);

        if (sdUser == null)
            await GetSightings();

        var sightings = await _sightingService.GetSightingsByUserIdAsync(sdUser.Id);
        return Ok(sightings);
    }
}