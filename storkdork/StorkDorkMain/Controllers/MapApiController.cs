using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models.DTO;
using System.Linq;

namespace StorkDork.Controllers;

[Route("api/map")]
[ApiController]
public class MappingApiController : ControllerBase
{
    private readonly ISightingService _sightingService;

    public MappingApiController(ISightingService sightingService)
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