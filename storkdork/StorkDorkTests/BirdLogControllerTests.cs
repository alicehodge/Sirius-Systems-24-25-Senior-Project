using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using NUnit.Framework;
using StorkDork.Controllers;
using StorkDorkMain.Controllers;
using StorkDorkMain.Data;
using StorkDorkMain.Models;
using Microsoft.AspNetCore.Identity;

namespace StorkDorkTests
{
    [TestFixture] 
    public class BirdLogControllerTests
    {
        private BirdLogController _controller;
        private Mock<StorkDorkContext> _mockContext;
        private Mock<UserManager<IdentityUser>> _mockUserManager;

        [SetUp] // Runs before each test
        public void Setup()
        {
            _mockContext = new Mock<StorkDorkContext>();

            var store = new Mock<IUserStore<IdentityUser>>();

            _mockUserManager = new Mock<UserManager<IdentityUser>>(store.Object,
                null, null, null, null, null, null, null, null);
            
            _controller = new BirdLogController(_mockContext.Object, _mockUserManager.Object);

            
        }
        [TearDown] // Runs after each test
        public void TearDown()
        {
            // Dispose of the controller if it implements IDisposable
            if (_controller is IDisposable disposable)
            {
                disposable.Dispose();
            }

            // Reset the controller to null (optional, but good practice)
            _controller = null;
        }
     

        [Test] // Marks this method as a test
        public async Task GetBirdSpecies_ReturnsEmptyList_WhenTermIsEmpty()
        {
            // Arrange
            var term = "";

            // Act
            var result = await _controller.GetBirdSpecies(term);

            // Assert
            Assert.IsInstanceOf<JsonResult>(result); // Check if the result is a JsonResult
            var jsonResult = result as JsonResult;
            var birds = jsonResult.Value as List<object>;
            Assert.IsEmpty(birds); // Check if the list is empty
        }

    }
}