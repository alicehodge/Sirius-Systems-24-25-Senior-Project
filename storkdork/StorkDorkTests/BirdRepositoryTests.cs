using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StorkDorkMain.DAL.Concrete;
using StorkDorkMain.Data;
using StorkDorkMain.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StorkDorkTests
{
    [TestFixture]
    public class BirdRepositoryTests
    {
        private Mock<StorkDorkDbContext> _mockContext;
        private Mock<DbSet<Bird>> _mockBirdSet;
        private BirdRepository _repository;
        private List<Bird> _birds;

        [SetUp]
        public void Setup()
        {
            _birds = new List<Bird>();
            _mockBirdSet = CreateMockDbSet(_birds);
            
            _mockContext = new Mock<StorkDorkDbContext>(new DbContextOptions<StorkDorkDbContext>());
            _mockContext.Setup(c => c.Birds).Returns(_mockBirdSet.Object);
            
            _repository = new BirdRepository(_mockContext.Object);
        }

        private Mock<DbSet<T>> CreateMockDbSet<T>(List<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockSet = new Mock<DbSet<T>>();

            mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());

            return mockSet;
        }

        [Test]
        public void GetAllOrders_ReturnsDistinctOrderedList()
        {
            // Arrange
            _birds.AddRange(new[]
            {
                new Bird { Order = "Passeriformes" },
                new Bird { Order = "Accipitriformes" },
                new Bird { Order = "Passeriformes" }  // Duplicate
            });

            // Act
            var orders = _repository.GetAllOrders().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(orders.Count, Is.EqualTo(2));
                Assert.That(orders[0], Is.EqualTo("Accipitriformes"));
                Assert.That(orders[1], Is.EqualTo("Passeriformes"));
            });
        }

        [Test]
        public async Task GetBirdsByOrder_ReturnsCorrectBirds()
        {
            // Arrange
            _birds.AddRange(new[]
            {
                new Bird { Order = "Passeriformes", CommonName = "American Robin" },
                new Bird { Order = "Accipitriformes", CommonName = "Red-tailed Hawk" },
                new Bird { Order = "Passeriformes", CommonName = "Blue Jay" }
            });

            // Act
            var birds = await _repository.GetBirdsByOrder("Passeriformes");

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(birds.Count(), Is.EqualTo(2));
                Assert.That(birds.All(b => b.Order == "Passeriformes"), Is.True);
                Assert.That(birds.Any(b => b.CommonName == "American Robin"), Is.True);
                Assert.That(birds.Any(b => b.CommonName == "Blue Jay"), Is.True);
            });
        }

        [Test]
        public void GetAllFamilies_ReturnsDistinctOrderedPairs()
        {
            // Arrange
            _birds.AddRange(new[]
            {
                new Bird { 
                    FamilyScientificName = "Turdidae", 
                    FamilyCommonName = "Thrushes" 
                },
                new Bird { 
                    FamilyScientificName = "Accipitridae", 
                    FamilyCommonName = "Hawks" 
                },
                new Bird { 
                    FamilyScientificName = "Turdidae", 
                    FamilyCommonName = "Thrushes" 
                }
            });

            // Act
            var families = _repository.GetAllFamilies().ToList();

            // Assert
            Assert.Multiple(() =>
            {
                Assert.That(families.Count, Is.EqualTo(2));
                Assert.That(families, Has.Some.Matches<(string ScientificName, string CommonName)>(
                    f => f.ScientificName == "Accipitridae" && f.CommonName == "Hawks"));
                Assert.That(families, Has.Some.Matches<(string ScientificName, string CommonName)>(
                    f => f.ScientificName == "Turdidae" && f.CommonName == "Thrushes"));
            });
        }

        [Test]
        public async Task GetBirdsByOrder_WithSorting_ReturnsSortedBirds()
        {
            // Arrange
            _birds.AddRange(new[]
            {
                new Bird { 
                    Order = "Passeriformes", 
                    CommonName = "Blue Jay", 
                    ScientificName = "Cyanocitta cristata" 
                },
                new Bird { 
                    Order = "Passeriformes", 
                    CommonName = "American Robin", 
                    ScientificName = "Turdus migratorius" 
                }
            });

            // Act - Test different sort orders
            var defaultSorted = await _repository.GetBirdsByOrder("Passeriformes", "name");
            var descSorted = await _repository.GetBirdsByOrder("Passeriformes", "name_desc");
            var scientificSorted = await _repository.GetBirdsByOrder("Passeriformes", "scientific");

            // Assert
            Assert.Multiple(() =>
            {
                // Default sort (A-Z by common name)
                Assert.That(defaultSorted.First().CommonName, Is.EqualTo("American Robin"));
                
                // Descending sort (Z-A by common name)
                Assert.That(descSorted.First().CommonName, Is.EqualTo("Blue Jay"));
                
                // Scientific name sort
                Assert.That(scientificSorted.First().ScientificName, Is.EqualTo("Cyanocitta cristata"));
            });
        }
    }
}