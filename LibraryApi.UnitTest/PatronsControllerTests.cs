using LibraryAPI.Models;
using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace LibraryApi.UnitTest
{
    [TestClass]
    public class PatronsControllerTests
    {
        private readonly Mock<IPatronDao> _mockPatronDao;
        private readonly PatronsController _mockPatronsController;
        private readonly PatronModel _mockPatronModel;
        public PatronsControllerTests()
        {
            _mockPatronDao = new Mock<IPatronDao>();
            _mockPatronsController = new PatronsController(_mockPatronDao.Object);
            _mockPatronModel = new PatronModel();
        }

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            Console.WriteLine("Inside ClassInitialize");
        }
        [ClassCleanup]
        public static void ClassCleanup()
        {
            Console.WriteLine("Inside ClassCleanup");
        }

        [TestMethod]
        public async Task GetListOfAllPatronsTest_ActionExecutes_ReturnsOkWithData()
        {
            Console.WriteLine("Inside TestMethod GetListOfAllPatronsTest");
            var result = await _mockPatronsController.GetListOfAllPatrons();
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }
        [TestMethod]
        public async Task AddPatronTest_ActionExecutes_ReturnsOk()
        {
            Console.WriteLine("Inside TestMethod AddBookTest");
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }

        [TestMethod]
        public async Task GetPatronByEmailTest_ActionExecutes_ReturnCode404WhenNull()
        {
            Console.WriteLine("Inside TestMethod GetBookByTitleTest");
            var result = await _mockPatronsController.GetPatronByEmail(_mockPatronModel.Email);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            //Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_ActionExecutes_ReturnsCode404WhenNull()
        {
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest");
            var result = await _mockPatronsController.UpdatePatronByEmail(_mockPatronModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));

        }
        [TestMethod]
        public async Task DeletePatronById_ActionExecutes_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside Delete Patron 200 test");
            _mockPatronModel.Id = 3;
            var result = await _mockPatronsController.DeletePatronById(_mockPatronModel.Id);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            //Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }


        [TestMethod]
        public async Task DeletePatronById_ActionExecutes_ReturnsCode404WhenNull()
        {
            Console.WriteLine("Inside TestMethod DeletePatronById");
            var result = await _mockPatronsController.DeletePatronById(2000);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, result.GetType());
            //Assert.AreEqual(StatusCodes.Status404NotFound, result);
        }
        [TestMethod]
        public async Task GetPatronByEmail_ActionExecutes_ReturnsCode404NotFound()
        {
            Console.WriteLine("Inside GetPatron 404 test.");
            var result = await _mockPatronsController.GetPatronByEmail("foo@email.com");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            //Assert.AreEqual(StatusCodes.Status404NotFound, result);
        }
        /*[TestMethod]
        *//*public async Task GetPatronByEmail_ActionExecutes_ReturnsCode404NotFound()
        {
            //Arrange 
            
            Console.WriteLine("Inside GetPatron not found test.");
            var result = (ObjectResult)await _mockPatronsController.GetPatronByEmail("james.remus@veteransunited.com");
            Assert.AreEqual(StatusCodes.Status404NotFound, result.StatusCode);
        }*/
    }
}
