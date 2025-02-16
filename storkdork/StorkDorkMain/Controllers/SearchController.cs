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
        public async Task<IActionResult> SearchBirds(string searchTerm, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("Index");
            }

            const int pageSize = 10;
            var birds = await _birdRepo.GetBirdsByName(searchTerm);
            var paginatedBirds = birds.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            var viewModel = new SearchResultsViewModel
            {
                Birds = paginatedBirds,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(birds.Count() / (double)pageSize),
                SearchTerm = searchTerm
            };

            return View("Index", viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> SearchPreview(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Json(new List<BirdPreview>());
            }

            var birds = await _birdRepo.GetBirdsByName(searchTerm);
            var results = birds.Select(b => new BirdPreview 
            { 
                CommonName = b.CommonName,
                ScientificName = b.ScientificName,
                Id = b.Id
            }).Take(5);
            
            return Json(results);
        }
    }
}