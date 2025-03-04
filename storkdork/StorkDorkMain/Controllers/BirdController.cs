using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.Models;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using System.Linq;
using System.Threading.Tasks;

namespace StorkDork.Controllers
{
    public class BirdController : Controller
    {
        private readonly IBirdRepository _birdRepo;

        public BirdController(IBirdRepository birdRepo)
        {
            _birdRepo = birdRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Details(int? id, string? categoryFilter = null, int page = 1)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bird = _birdRepo.FindById(id.Value);
            if (bird == null)
            {
                return NotFound();
            }

            var (relatedBirds, totalCount) = await _birdRepo.GetRelatedBirds(
                bird.SpeciesCode, 
                bird.ReportAs,
                categoryFilter,
                page);

            var viewModel = new BirdDetailsViewModel
            {
                Bird = bird,
                RelatedBirds = relatedBirds.Select(rb => new RelatedBirdViewModel 
                {
                    Bird = rb,
                    RelationType = rb.Category
                }).ToList(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalCount / 10.0),
                CategoryFilter = categoryFilter
            };

            return View(viewModel);
        }
    }
}