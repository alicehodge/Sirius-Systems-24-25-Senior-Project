using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Data;
using System.Linq;

namespace StorkDork.Controllers;

public class LeafletController : Controller
{
    private readonly StorkDorkContext _context;

    public LeafletController(StorkDorkContext context)
    {
        _context = context;
    }

    // Endpoint for fetching sightings for the map
    public IActionResult GetSightings()
    {
        var sightings = _context.Sightings
                .Where(s => s.Latitude != null && s.Longitude != null)
                .Select(s => new 
                {
                    s.Id,
                    s.Bird.CommonName,
                    s.Date,
                    s.Latitude,
                    s.Longitude,
                    s.Notes
                }).ToList();

            return Json(sightings);
    }

    public IActionResult Map()
    {
        return View();
    }
}