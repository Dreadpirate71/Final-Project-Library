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
        private IEnumerable<PatronModel> _patrons;
        private PatronModel? _patronModelMockNull;
        public PatronsControllerTests()
        {
            _mockPatronDao = new Mock<IPatronDao>();
            _mockPatronsController = new PatronsController(_mockPatronDao.Object);
            _mockPatronModel = new PatronModel() { Id = 1200, FirstName = "Jesus", LastName = "Christ", Email = "JesusLives@Heaven.com", StreetAddress = "1 Gold Street", City = "Clouds", State = "Joyous", PostalCode = "77777", PhoneNumber = "1234567890"};
            _patrons = new List<PatronModel>() { _mockPatronModel};
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
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllPatronsTest");
            _ = _mockPatronDao.Setup(patron => patron.GetListOfAllPatrons()).Returns(Task.FromResult(_patrons));
            //Act
            var result = await _mockPatronsController.GetListOfAllPatrons();
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetListOfAllPatronsTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllPatronsTest throws exception");
            _ = _mockPatronDao.Setup(patron => patron.GetListOfAllPatrons()).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.GetListOfAllPatrons();
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task AddPatronTest_ActionExecutes_ReturnsOk()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBookTest");
            //Act
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(OkResult));
        }
       
        [TestMethod]
        public async Task AddPatronTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddPatron throws exception");
            _mockPatronDao.Setup(patron => patron.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408")).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetPatronByEmailTest_RunsQuery_ReturnsOkObjectWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByEmaiTest Returns Ok");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_mockPatronModel));
            //Act
            var result = await _mockPatronsController.GetPatronByEmail(_mockPatronModel.Email) as OkObjectResult;
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, result.StatusCode);
        }

        [TestMethod]
        public async Task GetPatronByEmailTest_RunsQuery_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByEmailTest returns null message");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMockNull));
            //Act
            var result = await _mockPatronsController.GetPatronByEmail("email@email.com");
            var resultMessage = (result as ObjectResult).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Patron with that email does not exist!", (result as ObjectResult).Value);
        }
        [TestMethod]
        public async Task GetPatronByEmailTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByEmailTest throws exception");
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.GetPatronByEmail(_mockPatronModel.Email);
            var resultMessage = (result as ObjectResult).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }
        [TestMethod]
        public async Task UpdatePatronByEmailTest_RecordUpdated_ReturnsOKMessageWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest returns ok");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_mockPatronModel));
            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            //var resultMessage = (result as ObjectResult).Value;
            var resultStatusCode = result as OkObjectResult;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_RunsQuery_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest returns null message");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMockNull));
            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email","James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            var resultMessage = (result as ObjectResult).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Patron with that email does not exist!", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest throws exception");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_mockPatronModel));
            _mockPatronDao.Setup(patron => patron.UpdatePatronByEmail(It.IsAny<PatronModel>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            var resultMessage = (result as ObjectResult).Value;
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task DeletePatronById_ActionExecutes_ReturnsCode200WhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside Delete Patron 200 test");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_mockPatronModel));
            _ = _mockPatronDao.Setup(patron => patron.DeletePatronById(It.IsAny<int>()));
            //Act
            var result = await _mockPatronsController.DeletePatronById(_mockPatronModel.Id);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(StatusCodeResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as StatusCodeResult).StatusCode);
        }


        [TestMethod]
        public async Task DeletePatronById_ActionExecutes_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeletePatronById returns null message");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMockNull));
            //Act
            var result = await _mockPatronsController.DeletePatronById(2000);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Patron with that Id does not exist!", (result as ObjectResult).Value);
        }
        [TestMethod]
        public async Task DeletePatronByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeletePatronByIdTest throws exception");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_mockPatronModel));
            _mockPatronDao.Setup(patron => patron.DeletePatronById(It.IsAny<int>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.DeletePatronById(_mockPatronModel.Id);
           
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }
        [TestMethod]
        public async Task GetPatronById_ExecutesQuery_ReturnsOkObjectResult()
        {
            //Arrange
            Console.WriteLine("Inside GetPatronByIdTest returns Ok result");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_mockPatronModel));
            //Act
            var result = await _mockPatronsController.GetPatronById(_mockPatronModel.Id);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);

        }
        [TestMethod]
        public async Task GetPatronByIdTest_ExecutesQuery_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside GetPatronByIdTest returns null message.");
            _ = _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMockNull));
            //Act
            var result = await _mockPatronsController.GetPatronById(_mockPatronModel.Id);
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Patron with that Id number does not exist!", (result as ObjectResult).Value);
        }
        [TestMethod]
        public async Task GetPatronByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByIdTest throws exception");
            //_ = _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_mockPatronModel));
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.GetPatronById(_mockPatronModel.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
        }
    }
}
