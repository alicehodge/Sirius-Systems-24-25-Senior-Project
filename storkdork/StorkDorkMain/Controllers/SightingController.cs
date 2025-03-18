using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models.DTO;

namespace StorkDorkMain.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SightingController : ControllerBase
{
        private readonly ISightingService _sightingService;

        public SightingController(ISightingService sightingService)
        {
            _sightingService = sightingService;
        }

        [HttpPost("UpdateLocation")]
        public async Task<IActionResult> UpdateSightingLocation([FromBody] LocationUpdate update)
        {
            if (update == null || update.SightingId <= 0)
                return BadRequest("Invalid request data.");

            try
            {
                await _sightingService.UpdateSightingLocationAsync(update.SightingId, update.Country, update.Subdivision);
                return Ok("Location updates successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Sighting not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }
}