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
using System.Globalization;
using System.Numerics;

namespace LibraryApi.UnitTest
{
    [TestClass]
    public class PatronsControllerTests
    {
        private readonly Mock<IPatronDao> _mockPatronDao;
        private readonly Mock<IStaffDao> _mockStaffDao;
        private readonly Mock<PatronModel> _mockPatronModel;
        private readonly PatronsController _mockPatronsController;
        private readonly PatronModel _patronModel;
        private readonly StaffModel _mockStaffModel;
        private IEnumerable<PatronModel> _patrons;
        private PatronModel? _patronModelMockNull;
        public PatronsControllerTests()
        {

            _mockPatronDao = new Mock<IPatronDao>();
            _mockStaffDao = new Mock<IStaffDao>();
            _mockPatronModel = new Mock<PatronModel>();
            _mockPatronsController = new PatronsController(_mockPatronDao.Object, _mockStaffDao.Object);
            _patronModel = new PatronModel() { Id = 1200, FirstName = "Jesus", LastName = "Christ", Email = "JesusLives@Heaven.com", StreetAddress = "1 Gold Street", City = "Clouds", 
                                                  State = "Joyous", PostalCode = "77777", PhoneNumber = "1234567890", BooksHistory = null };
            _patrons = new List<PatronModel>() { _patronModel };
            _mockStaffModel = new StaffModel() { Id = 1103, FirstName = "James", LastName = "Remus", PhoneNumber = "5738087408", Position = "Admin" };
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
        public async Task GetPatronTest_ByIdExecutes_ReturnsOkWithPatron()
        {
            //Arrange 
            Console.WriteLine("Inside GetPatronTest returns Ok.");
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.GetPatron(3, null, null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
            Assert.AreEqual(_patronModel, (result as ObjectResult).Value); 
        }

        [TestMethod]
        public async Task GetPatronTest_ByEmailExecutes_ReturnsOkWithPatrons()
        {
            //Arrange 
            Console.WriteLine("Inside GetPatronTest returns Ok.");
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.GetPatron(0, "email@email.com", null, null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
            Assert.AreEqual(_patronModel, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetPatronTest_ByPhoneNumberExecutes_ReturnsOkWithPatrons()
        {
            //Arrange 
            Console.WriteLine("Inside GetPatronTest returns Ok.");
            _mockPatronDao.Setup(patron => patron.GetPatronByPhoneNumber(It.IsAny<string>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.GetPatron(0, null, "1112223333", null);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
            Assert.AreEqual(_patronModel, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetPatronTest_ByLastNameExecutes_ReturnsOkWithPatrons()
        {
            //Arrange 
            Console.WriteLine("Inside GetPatronTest returns Ok.");
            _mockPatronDao.Setup(patron => patron.GetPatronByLastName(It.IsAny<string>())).Returns(Task.FromResult(_patrons));

            //Act
            var result = await _mockPatronsController.GetPatron(0, null, null, "lastName");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
            Assert.AreEqual(_patrons, (result as ObjectResult).Value);
        }
        
        [TestMethod]
        public async Task AddPatronTest_ActionExecutes_ReturnsOk()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBookTest");
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("New Patron created.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
        }
       
        [TestMethod]
        public async Task AddPatronTest_RunsEmailQuery_ReturnsErrorForEmail()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddPatron throws exception");
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(false));

            //Act
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("That email is already in use. Please use a different email.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        [DataRow("123@1456523")]
        [DataRow("[253]2305")]
        [DataRow("J25364589")]
        public void CheckPhoneNumberTest_RunCheck_ReturnsMessageWhenFalse(string phoneNumber)
        {
            //Arrange
            Console.WriteLine("Inside CheckPhoneTest False");
            //Act
            var result = _patronModel.CheckPhoneNumber(phoneNumber);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is bool);
            Assert.AreEqual(false, result);
        }

        [TestMethod]
        [DataRow("1Kris.yahoo.com")]
        [DataRow("u@yahoocom")]
        [DataRow("jjjjjkkkkk")]
        public void IsValidEmailRegEx_RunCheck_ReturnsMessageWhenFalse(string email)
        {
            //Arrange
            Console.WriteLine("Inside Valid Email Returns False and message.");

            //Act
            var result = _patronModel.IsValidEmailRegEx(email);

            //Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(false, result);
        }
        
        [TestMethod]
        public async Task AddPatronTest_RunCheckPhone_ReturnsMessageWhenFalse()
        {
            //Arrange 
            Console.WriteLine("Inside AddPatronTest check phone returns message when false.");
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "123@1234568");
          
            //Assert
            Assert.IsNotNull (result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("The phone number entered is not valid!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_RecordUpdated_ReturnsOKMessageWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest returns ok");
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(update => update.UpdatePatronById(It.IsAny<PatronModel>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", 
                                                                            "Columbia", "MO", "65203", "5738087408");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Patron has been updated!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_RunsQuery_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest returns null message");
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(update => update.UpdatePatronById(It.IsAny<PatronModel>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMockNull));

            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.",
                                                                            "Columbia", "MO", "65203", "5738087408");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("No patron with that email exists.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest throws exception");
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.UpdatePatronById(It.IsAny<PatronModel>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408");
            var resultMessage = (result as ObjectResult).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }
    
        [TestMethod]
        public async Task DeletePatronById_ActionExecutes_Returns200MessageWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside Delete Patron 200 test");
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.DeletePatronById(It.IsAny<int>()));

            //Act
            var result = await _mockPatronsController.DeletePatronById(_patronModel.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Patron has been deleted.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeletePatronById_ActionExecutes_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeletePatronById returns null message");
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMockNull));

            //Act
            var result = await _mockPatronsController.DeletePatronById(5);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Patron with that Id does not exist!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeletePatronByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod DeletePatronByIdTest throws exception");
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.DeletePatronById(It.IsAny<int>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.DeletePatronById(_patronModel.Id);
           
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }
    }
}
