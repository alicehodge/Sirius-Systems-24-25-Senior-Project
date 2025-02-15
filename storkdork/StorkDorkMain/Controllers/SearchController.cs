using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDork.Controllers
{
    public class SearchController : Controller
    {
        private readonly IBirdRepository _birdRepo;

        public SearchController(IBirdRepository birdRepo)
        {
            _birdRepo = birdRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> SearchBirds(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("Index");
            }

            var birds = await _birdRepo.GetBirdsByName(searchTerm);
            return View("Index", birds);
        }
    }
}