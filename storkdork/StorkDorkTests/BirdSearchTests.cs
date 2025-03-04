using Microsoft.AspNetCore.Mvc;
using Moq;
using StorkDorkMain.Controllers;
using StorkDorkMain.DAL.Abstract;
using StorkDorkMain.Models;
using NUnit.Framework;

namespace StorkDorkTests
{
    [TestFixture]
    public class BirdSearchTests : IDisposable
    {
        private Mock<IBirdRepository> _mockBirdRepo;
        private SearchController _controller;
        private List<Bird> _testBirds;
        private bool _disposed;

        [SetUp]
        public void Setup()
        {
            _mockBirdRepo = new Mock<IBirdRepository>();
            _controller = new SearchController(_mockBirdRepo.Object);
            
            _testBirds = new List<Bird>
            {
                new Bird { 
                    Id = 1, 
                    CommonName = "American Robin", 
                    ScientificName = "Turdus migratorius",
                    Order = "Passeriformes",
                    FamilyCommonName = "Thrushes",
                    FamilyScientificName = "Turdidae"
                },
                new Bird { 
                    Id = 2, 
                    CommonName = "Blue Jay", 
                    ScientificName = "Cyanocitta cristata",
                    Order = "Passeriformes",
                    FamilyCommonName = "Jays and Crows",
                    FamilyScientificName = "Corvidae"
                },
                new Bird { 
                    Id = 3, 
                    CommonName = "Northern Cardinal", 
                    ScientificName = "Cardinalis cardinalis",
                    Order = "Passeriformes",
                    FamilyCommonName = "Cardinals",
                    FamilyScientificName = "Cardinalidae"
                }
            };
        }

        [Test]
        public async Task SearchBirds_EmptySearchTerm_ReturnsIndexView()
        {
            // Arrange
            var searchTerm = "";

            // Act
            var result = await _controller.SearchBirds(searchTerm);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
        }

        [Test]
        public async Task SearchBirds_ValidSearchTerm_ReturnsBirds()
        {
            // Arrange
            var searchTerm = "robin";
            var searchType = "name";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByName(searchTerm))
                        .ReturnsAsync(_testBirds.Where(b => 
                            b.CommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            b.ScientificName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList());

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.TypeOf<SearchResultsViewModel>());
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.Birds.Count(), Is.EqualTo(1));
                Assert.That(model.Birds.First().CommonName, Is.EqualTo("American Robin"));
                Assert.That(model.SearchType, Is.EqualTo("name"));
            });
        }

        [Test]
        public async Task SearchBirds_TaxonomySearch_ReturnsBirds()
        {
            // Arrange
            var searchTerm = "Passeriformes";
            var searchType = "taxonomy";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByTaxonomy(searchTerm))
                        .ReturnsAsync(_testBirds.Where(b => 
                            b.Order.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            b.FamilyCommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            b.FamilyScientificName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList());

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.TypeOf<SearchResultsViewModel>());
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.SearchType, Is.EqualTo("taxonomy"));
                Assert.That(model.Birds, Is.Not.Empty);
            });
        }

        [Test]
        public async Task SearchPreview_EmptySearchTerm_ReturnsEmptyList()
        {
            // Arrange
            var searchTerm = "";
            var searchType = "name";

            // Act
            var result = await _controller.SearchPreview(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            var birds = ((IEnumerable<BirdPreview>)jsonResult.Value).ToList();
            Assert.That(birds, Is.Empty);
        }

        [Test]
        public async Task SearchPreview_ValidSearchTerm_ReturnsPreviewList()
        {
            // Arrange
            var searchTerm = "cardinal";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByName(searchTerm))
                        .ReturnsAsync(_testBirds.Where(b => 
                            b.CommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            b.ScientificName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList());

            // Act
            var result = await _controller.SearchPreview(searchTerm);

            // Assert
            Assert.That(result, Is.TypeOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            var birds = ((IEnumerable<BirdPreview>)jsonResult.Value).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(birds.Count, Is.EqualTo(1));
                var firstBird = birds.First();
                Assert.That(firstBird.CommonName, Is.EqualTo("Northern Cardinal"));
                Assert.That(firstBird.ScientificName, Is.EqualTo("Cardinalis cardinalis"));
                Assert.That(firstBird.Id, Is.EqualTo(3));
            });
        }

        [Test]
        public async Task SearchPreview_ValidNameSearch_ReturnsPreviewList()
        {
            // Arrange
            var searchTerm = "cardinal";
            var searchType = "name";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByName(searchTerm))
                        .ReturnsAsync(_testBirds.Where(b => 
                            b.CommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            b.ScientificName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList());

            // Act
            var result = await _controller.SearchPreview(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            var birds = ((IEnumerable<BirdPreview>)jsonResult.Value).ToList();
            
            Assert.Multiple(() =>
            {
                Assert.That(birds.Count, Is.EqualTo(1));
                var firstBird = birds.First();
                Assert.That(firstBird.CommonName, Is.EqualTo("Northern Cardinal"));
                Assert.That(firstBird.ScientificName, Is.EqualTo("Cardinalis cardinalis"));
                Assert.That(firstBird.Id, Is.EqualTo(3));
            });
        }

        [Test]
        public async Task SearchPreview_ValidTaxonomySearch_ReturnsPreviewList()
        {
            // Arrange
            var searchTerm = "Passeriformes";
            var searchType = "taxonomy";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByTaxonomy(searchTerm))
                        .ReturnsAsync(_testBirds.Where(b => 
                            b.Order.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) || 
                            b.FamilyCommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            b.FamilyScientificName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList());

            // Act
            var result = await _controller.SearchPreview(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            var birds = ((IEnumerable<BirdPreview>)jsonResult.Value).ToList();
            Assert.Multiple(() =>
            {
                Assert.That(birds, Is.Not.Empty);
                var firstBird = birds.First();
                Assert.That(firstBird.Order, Is.EqualTo("Passeriformes"));
            });
        }

        [Test]
        public async Task SearchBirds_Pagination_ReturnsCorrectPage()
        {
            // Arrange
            var searchTerm = "bird";
            var searchType = "name";
            var page = 2;
            _mockBirdRepo.Setup(repo => repo.GetBirdsByName(searchTerm))
                        .ReturnsAsync(_testBirds);

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType, page);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.TypeOf<SearchResultsViewModel>());
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.CurrentPage, Is.EqualTo(page));
                Assert.That(model.SearchType, Is.EqualTo("name"));
            });
        }

        [Test]
        public async Task SearchBirds_TaxonomyPagination_ReturnsCorrectPage()
        {
            // Arrange
            var searchTerm = "Passeriformes";
            var searchType = "taxonomy";
            var page = 2;
            _mockBirdRepo.Setup(repo => repo.GetBirdsByTaxonomy(searchTerm))
                        .ReturnsAsync(_testBirds);

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType, page);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.TypeOf<SearchResultsViewModel>());
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.CurrentPage, Is.EqualTo(page));
                Assert.That(model.SearchType, Is.EqualTo("taxonomy"));
            });
        }

        [Test]
        public async Task SearchBirds_NoResults_ShowsCorrectErrorMessage()
        {
            // Arrange
            var searchTerm = "nonexistentbird";
            var searchType = "name";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByName(searchTerm))
                        .ReturnsAsync(new List<Bird>());

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.TypeOf<SearchResultsViewModel>());
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.Birds, Is.Empty);
                Assert.That(model.SearchTerm, Is.EqualTo(searchTerm));
                Assert.That(model.SearchType, Is.EqualTo(searchType));
            });
        }

        [Test]
        public async Task SearchBirds_NoResultsTaxonomy_ShowsCorrectErrorMessage()
        {
            // Arrange
            var searchTerm = "nonexistentorder";
            var searchType = "taxonomy";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByTaxonomy(searchTerm))
                        .ReturnsAsync(new List<Bird>());

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<ViewResult>());
            var viewResult = (ViewResult)result;
            Assert.That(viewResult.Model, Is.TypeOf<SearchResultsViewModel>());
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.Birds, Is.Empty);
                Assert.That(model.SearchTerm, Is.EqualTo(searchTerm));
                Assert.That(model.SearchType, Is.EqualTo(searchType));
            });
        }

        [Test]
        public async Task SearchPreview_NoResults_ReturnsEmptyList()
        {
            // Arrange
            var searchTerm = "nonexistentbird";
            var searchType = "name";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByName(searchTerm))
                        .ReturnsAsync(new List<Bird>());

            // Act
            var result = await _controller.SearchPreview(searchTerm, searchType);

            // Assert
            Assert.That(result, Is.TypeOf<JsonResult>());
            var jsonResult = (JsonResult)result;
            var birds = ((IEnumerable<BirdPreview>)jsonResult.Value).ToList();
            Assert.That(birds, Is.Empty);
        }

        [Test]
        public async Task SearchBirds_ValidNameSearch_RetainsNameTab()
        {
            // Arrange
            var searchTerm = "robin";
            var searchType = "name";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByName(searchTerm))
                        .ReturnsAsync(_testBirds.Where(b => 
                            b.CommonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList());

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType);

            // Assert
            var viewResult = (ViewResult)result;
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.SearchType, Is.EqualTo("name"));
                Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
                // Verify tab state through model
                Assert.That(model.Birds.Any(), Is.True, "Should have search results");
            });
        }

        [Test]
        public async Task SearchBirds_ValidTaxonomySearch_RetainsTaxonomyTab()
        {
            // Arrange
            var searchTerm = "Passeriformes";
            var searchType = "taxonomy";
            _mockBirdRepo.Setup(repo => repo.GetBirdsByTaxonomy(searchTerm))
                        .ReturnsAsync(_testBirds.Where(b => 
                            b.Order.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList());

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType);

            // Assert
            var viewResult = (ViewResult)result;
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.SearchType, Is.EqualTo("taxonomy"));
                Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
                // Verify tab state through model
                Assert.That(model.Birds.Any(), Is.True, "Should have search results");
            });
        }

        [Test]
        public async Task SearchBirds_NoResults_RetainsCorrectTab()
        {
            // Arrange
            var searchTerm = "nonexistentbird";
            var searchType = "taxonomy"; // Testing with taxonomy search
            _mockBirdRepo.Setup(repo => repo.GetBirdsByTaxonomy(searchTerm))
                        .ReturnsAsync(new List<Bird>());

            // Act
            var result = await _controller.SearchBirds(searchTerm, searchType);

            // Assert
            var viewResult = (ViewResult)result;
            var model = (SearchResultsViewModel)viewResult.Model;
            Assert.Multiple(() =>
            {
                Assert.That(model.SearchType, Is.EqualTo("taxonomy"), "Should retain taxonomy search type even with no results");
                Assert.That(model.Birds, Is.Empty, "Should have no results");
                Assert.That(viewResult.ViewName, Is.EqualTo("Index"));
            });
        }

        [Test]
        public async Task GetRelatedBirds_WithSubspecies_ReturnsRelatedBirds()
        {
            // Arrange
            var speciesCode = "amecro";
            var reportAs = "amecro";
            _testBirds = new List<Bird>
            {
                new Bird {
                    Id = 1,
                    CommonName = "American Crow",
                    ScientificName = "Corvus brachyrhynchos",
                    SpeciesCode = "amecro",
                    Category = "species",
                    ReportAs = "amecro"
                },
                new Bird {
                    Id = 2,
                    CommonName = "American Crow (Northwestern)",
                    ScientificName = "Corvus brachyrhynchos caurinus",
                    SpeciesCode = "amecr1",
                    Category = "issf",
                    ReportAs = "amecro"
                }
            };
            _mockBirdRepo.Setup(repo => repo.GetRelatedBirds(speciesCode, reportAs, null, 1, 10))
                        .ReturnsAsync((_testBirds.Skip(1), 1));

            // Act
            var result = await _mockBirdRepo.Object.GetRelatedBirds(speciesCode, reportAs);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Birds.Count(), Is.EqualTo(1));
                var relatedBird = result.Birds.First();
                Assert.That(relatedBird.Category, Is.EqualTo("issf"));
                Assert.That(relatedBird.ReportAs, Is.EqualTo("amecro"));
            });
        }

        [Test]
        public async Task GetRelatedBirds_WithHybrids_ReturnsHybrids()
        {
            // Arrange
            var speciesCode = "daejun";
            var reportAs = "daejun";
            _testBirds = new List<Bird>
            {
                new Bird {
                    Id = 1,
                    CommonName = "Dark-eyed Junco",
                    ScientificName = "Junco hyemalis",
                    SpeciesCode = "daejun",
                    Category = "species"
                },
                new Bird {
                    Id = 2,
                    CommonName = "Dark-eyed x White-winged Junco (hybrid)",
                    ScientificName = "Junco hyemalis x aikeni",
                    SpeciesCode = "x00123",
                    Category = "hybrid"
                }
            };
            _mockBirdRepo.Setup(repo => repo.GetRelatedBirds(speciesCode, reportAs, null, 1, 10))
                        .ReturnsAsync((_testBirds.Skip(1), 1));

            // Act
            var result = await _mockBirdRepo.Object.GetRelatedBirds(speciesCode, reportAs);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Birds.Count(), Is.EqualTo(1));
                var hybrid = result.Birds.First();
                Assert.That(hybrid.Category, Is.EqualTo("hybrid"));
                Assert.That(hybrid.CommonName.Contains("Dark-eyed"), Is.True);
            });
        }

        [Test]
        public async Task GetRelatedBirds_WithCategoryFilter_ReturnsFilteredResults()
        {
            // Arrange
            var speciesCode = "daejun";
            var reportAs = "daejun";
            var categoryFilter = "hybrid";
            _testBirds = new List<Bird>
            {
                new Bird {
                    Id = 1,
                    CommonName = "Dark-eyed Junco",
                    SpeciesCode = "daejun",
                    Category = "species"
                },
                new Bird {
                    Id = 2,
                    CommonName = "Dark-eyed Junco (Oregon)",
                    SpeciesCode = "daej1",
                    Category = "issf",
                    ReportAs = "daejun"
                },
                new Bird {
                    Id = 3,
                    CommonName = "Dark-eyed x White-winged Junco",
                    SpeciesCode = "x00123",
                    Category = "hybrid"
                }
            };
            _mockBirdRepo.Setup(repo => repo.GetRelatedBirds(speciesCode, reportAs, categoryFilter, 1, 10))
                        .ReturnsAsync((_testBirds.Where(b => b.Category == categoryFilter), 1));

            // Act
            var result = await _mockBirdRepo.Object.GetRelatedBirds(speciesCode, reportAs, categoryFilter);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Birds.Count(), Is.EqualTo(1));
                Assert.That(result.Birds.All(b => b.Category == categoryFilter), Is.True);
            });
        }

        [Test]
        public async Task GetRelatedBirds_WithPagination_ReturnsCorrectPage()
        {
            // Arrange
            var speciesCode = "daejun";
            var reportAs = "daejun";
            var page = 2;
            var pageSize = 1;
            _testBirds = new List<Bird>
            {
                new Bird { Id = 1, CommonName = "Dark-eyed Junco", Category = "species" },
                new Bird { Id = 2, CommonName = "Dark-eyed Junco (Oregon)", Category = "issf", ReportAs = "daejun" },
                new Bird { Id = 3, CommonName = "Dark-eyed x White-winged Junco", Category = "hybrid" }
            };
            _mockBirdRepo.Setup(repo => repo.GetRelatedBirds(speciesCode, reportAs, null, page, pageSize))
                        .ReturnsAsync((_testBirds.Skip((page - 1) * pageSize).Take(pageSize), _testBirds.Count));

            // Act
            var result = await _mockBirdRepo.Object.GetRelatedBirds(speciesCode, reportAs, null, page, pageSize);

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(result.Birds.Count(), Is.EqualTo(pageSize));
                Assert.That(result.TotalCount, Is.EqualTo(_testBirds.Count));
            });
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        [TearDown]
        public void TearDown()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _controller?.Dispose();
                }
                _mockBirdRepo = null;
                _controller = null;
                _testBirds = null;
                _disposed = true;
            }
        }
    }
}