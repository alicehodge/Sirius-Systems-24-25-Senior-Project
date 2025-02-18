using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models.DTO;
using System.Linq;

namespace StorkDorkMain.Controllers;

[Route("api/map")]
[ApiController]
public class MapApiController : ControllerBase
{
    private readonly ISightingService _sightingService;

    public MapApiController(ISightingService sightingService)
    {
        _sightingService = sightingService;
    }

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

    [HttpGet]
    [Route("GetSightings/{userId}")]
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
}