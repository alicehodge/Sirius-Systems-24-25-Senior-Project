using Microsoft.AspNetCore.Mvc;
using StorkDork.Models;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.DAL.Concrete;
using StorkDorkMain.Models;

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
        public Task<IActionResult> Details(int id)
        {
            var bird = _birdRepo.FindById(id);
            if (bird == null)
            {
                return Task.FromResult<IActionResult>(NotFound());
            }

            var viewModel = new BirdDetailsViewModel
            {
                CommonName = bird.CommonName,
                ScientificName = bird.ScientificName,
                Order = bird.Order,
                FamilyCommonName = bird.FamilyCommonName,
                FamilyScientificName = bird.FamilyScientificName,
                Range = bird.Range
            };

            return Task.FromResult<IActionResult>(View(viewModel));
        }
    }
}