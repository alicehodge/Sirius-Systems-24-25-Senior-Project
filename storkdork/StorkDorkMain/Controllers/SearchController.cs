using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDorkMain.Controllers
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
        [Route("search/birds")]
        public async Task<IActionResult> SearchBirds(string searchTerm, string searchType = "name", int page = 1)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return View("Index");
            }

            const int pageSize = 10;
            var birds = searchType == "taxonomy" 
                ? await _birdRepo.GetBirdsByTaxonomy(searchTerm)
                : await _birdRepo.GetBirdsByName(searchTerm);

            var paginatedBirds = birds.Skip((page - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToList();

            var viewModel = new SearchResultsViewModel
            {
                Birds = paginatedBirds,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(birds.Count() / (double)pageSize),
                SearchTerm = searchTerm,
                SearchType = searchType
            };

            return View("Index", viewModel);
        }

        [HttpGet]
        [Route("search/preview")]
        public async Task<IActionResult> SearchPreview(string searchTerm, string searchType = "name")
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Json(new List<BirdPreview>());
            }

            var birds = searchType == "taxonomy" 
                ? await _birdRepo.GetBirdsByTaxonomy(searchTerm)
                : await _birdRepo.GetBirdsByName(searchTerm);

            var results = birds.Select(b => new BirdPreview 
            { 
                CommonName = b.CommonName,
                ScientificName = b.ScientificName,
                Id = b.Id,
                Order = b.Order,
                FamilyCommonName = b.FamilyCommonName,
                FamilyScientificName = b.FamilyScientificName
            }).Take(5);
            
            return Json(results);
        }
    }
}