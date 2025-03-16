using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Models;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using System.Linq;
using System.Threading.Tasks;

namespace StorkDork.Controllers;

public class MilestoneController : Controller
{

    public MilestoneController()
    {

    }

    public IActionResult Milestone()
    {
        return View();
    }
}