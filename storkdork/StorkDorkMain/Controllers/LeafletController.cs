using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Data;
using System.Linq;

namespace StorkDorkMain.Controllers;

public class LeafletController : Controller
{
    private readonly ISightingService _sightingservice;

    public LeafletController(ISightingService sightingService)
    {
        _sightingservice = sightingService;
    }

    public IActionResult Map()
    {
        return View();
    }
}