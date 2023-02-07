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
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203","5738087408");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task GetPatronByEmaiTest_ActionExecutes_ReturnsOkWithData()
        {
            Console.WriteLine("Inside TestMethod GetBookByTitleTest");
            var result = await _mockPatronsController.GetPatronByEmail("james.remus@veteransunited.com");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_ActionExecutes_ReturnsCode204WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest");
            var result = await _mockPatronsController.UpdatePatronByEmail(_mockPatronModel);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, result.GetType());
        }
        [TestMethod]
        public async Task DeletePatronById_ActionExecutes_ReturnsCode200WhenSuccessful()
        {
            Console.WriteLine("Inside TestMethod DeleteBookById");
            var result = _mockPatronsController.DeletePatronById(_mockPatronModel.Id);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, result.GetType());
        }
    }
}
