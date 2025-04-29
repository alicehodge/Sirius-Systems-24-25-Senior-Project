using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;

namespace StorkDork.Controllers
{
    public class TaxonomyController : Controller
    {
        private readonly IBirdRepository _birdRepo;
        private readonly ILogger<TaxonomyController> _logger;

        public TaxonomyController(IBirdRepository birdRepo, ILogger<TaxonomyController> logger)
        {
            _birdRepo = birdRepo;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                _logger.LogInformation("Fetching taxonomy data for index page");
                
                var orders = _birdRepo.GetAllOrders();
                var families = _birdRepo.GetAllFamilies();
                
                _logger.LogInformation($"Found {orders.Count()} orders and {families.Count()} families");

                var viewModel = new TaxonomyViewModel
                {
                    Orders = orders,
                    Families = families
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading taxonomy index page");
                throw;
            }
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
                "name" => birds.OrderBy(b => b.CommonName),
                "name_desc" => birds.OrderByDescending(b => b.CommonName),
                "scientific" => birds.OrderBy(b => b.ScientificName),
                "scientific_desc" => birds.OrderByDescending(b => b.ScientificName),
                _ => birds.OrderBy(b => b.CommonName)
            };
        }
    }
}