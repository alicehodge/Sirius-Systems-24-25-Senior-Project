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
        var sightings = await _sightingService.GetSightingsAsync();
        return Ok(sightings);
    }
}