using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Models;
using Microsoft.EntityFrameworkCore;
using StorkDorkMain.Data;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Authorization;

namespace StorkDorkMain.Controllers;

public class UserSettingsController : Controller
{
    private readonly StorkDorkContext _context;
    private readonly ILogger<UserSettingsController> _logger;

    public UserSettingsController(ILogger<UserSettingsController> logger)
    {
        _logger = logger;
    }

    public IActionResult Settings()
    {
        return View();
    }
    
}