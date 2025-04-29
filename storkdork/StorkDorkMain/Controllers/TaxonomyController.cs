using Microsoft.AspNetCore.Mvc;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDork.Controllers
{
    public class TaxonomyController : Controller
    {
        private readonly IBirdRepository _birdRepo;

        public TaxonomyController(IBirdRepository birdRepo)
        {
            _birdRepo = birdRepo;
        }

        public IActionResult Index()
        {
            var viewModel = new TaxonomyViewModel
            {
                Orders = _birdRepo.GetAllOrders(),
                Families = _birdRepo.GetAllFamilies()
            };
            return View(viewModel);
        }

        public async Task<IActionResult> Order(string order, string sortOrder = "name")
        {
            if (string.IsNullOrEmpty(order))
                return NotFound();

            var birds = await _birdRepo.GetBirdsByOrder(order);
            birds = ApplySort(birds, sortOrder);

            var viewModel = new TaxonomyListViewModel
            {
                Title = $"Birds in Order {order}",
                Birds = birds,
                CurrentSort = sortOrder,
                TaxonomicGroup = order
            };

            return View("BirdList", viewModel);
        }

        public async Task<IActionResult> Family(string family, string sortOrder = "name")
        {
            if (string.IsNullOrEmpty(family))
                return NotFound();

            var birds = await _birdRepo.GetBirdsByFamily(family);
            birds = ApplySort(birds, sortOrder);

            var viewModel = new TaxonomyListViewModel
            {
                Title = $"Birds in Family {family}",
                Birds = birds,
                CurrentSort = sortOrder,
                TaxonomicGroup = family
            };

            return View("BirdList", viewModel);
        }

        private IEnumerable<Bird> ApplySort(IEnumerable<Bird> birds, string sortOrder)
        {
            return sortOrder switch
            {
                "name_desc" => birds.OrderByDescending(b => b.CommonName),
                "scientific" => birds.OrderBy(b => b.ScientificName),
                "scientific_desc" => birds.OrderByDescending(b => b.ScientificName),
                _ => birds.OrderBy(b => b.CommonName)
            };
        }
    }
}