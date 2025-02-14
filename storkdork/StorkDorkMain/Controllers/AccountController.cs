using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDork.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchBirds()
        {
            return View("Index");
        }
    }
}