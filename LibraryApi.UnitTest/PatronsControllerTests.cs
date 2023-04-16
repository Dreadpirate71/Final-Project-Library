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
                                                  State = "Joyous", PostalCode = "77777", PhoneNumber = "1234567890", Password = "HeLives!", AccountLock = "No" };
            _patrons = new List<PatronModel>() { _patronModel };
            _mockStaffModel = new StaffModel() { Id = 1103, FirstName = "James", LastName = "Remus", PhoneNumber = "5738087408", Position = "Admin", Password = "IMAdmin!" };
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
            Console.WriteLine("Inside TestMethod You need to have proper admin credentials to complete this task!");
            _mockPatronDao.Setup(patron => patron.GetListOfAllPatrons()).Returns(Task.FromResult(_patrons));
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockPatronsController.GetListOfAllPatrons(1, "password");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
            Assert.AreEqual(_patrons, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetListOfAllPatronsTest_ActionExecutes_ReturnsAdminErrorWhenFalse()
        {
            //Arrange
            Console.WriteLine("Inside GetListOfAllPatronsTest returns admin error message.");
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            //Act
            var result = await _mockPatronsController.GetListOfAllPatrons(1, "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status403Forbidden, (result as ObjectResult).StatusCode);
            Assert.AreEqual("You need to have proper admin credentials to complete this task!", (result as ObjectResult).Value);
            
        }

        [TestMethod]
        public async Task GetListOfAllPatronsTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetListOfAllPatronsTest throws exception");
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetListOfAllPatrons()).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.GetListOfAllPatrons(1, "password");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task AddPatronTest_ActionExecutes_ReturnsOk()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddBookTest");
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408", "Password", "Password");

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
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408", "Password", "Password");
            
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
            var result = await _mockPatronsController.AddPatron("James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "123@1234568", "Password", "Password");
          
            //Assert
            Assert.IsNotNull (result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("The phone number entered is not valid!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetPatronByEmailTest_RunsQuery_ReturnsOkObjectWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByEmaiTest Returns Ok");
            _mockPatronDao.Setup(check => check.CheckPatronCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.GetPatronByEmail(_patronModel.Email, _patronModel.Password);
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
            Assert.AreEqual(_patronModel, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetPatronByEmailTest_RunsQuery_ReturnsMessageWhenFalse()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByEmailTest returns null message");
            _mockPatronDao.Setup(check => check.CheckPatronCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMockNull));

            //Act
            var result = await _mockPatronsController.GetPatronByEmail("email@email.com", "patronPassword");
            var resultMessage = (result as ObjectResult).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Patron with that email and password does not exist!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetPatronByEmailTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByEmailTest throws exception");
            _mockPatronDao.Setup(check => check.CheckPatronCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.GetPatronByEmail(_patronModel.Email, _patronModel.Password);
            var resultMessage = (result as ObjectResult).Value;

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_RecordUpdated_ReturnsOKMessageWhenSuccessful()
        {
            //Arrange 
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest returns ok");
            _mockPatronDao.Setup(check => check.CheckPatronCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(update => update.UpdatePatronById(It.IsAny<PatronModel>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "patronPassword", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", 
                                                                            "Columbia", "MO", "65203", "5738087408", "password", "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Patron has been updated!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_RunsQuery_ReturnsMessageWhenFalse()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest returns null message");
            _mockPatronDao.Setup(check => check.CheckPatronCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(update => update.UpdatePatronById(It.IsAny<PatronModel>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModelMockNull));

            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "patronPassword", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.",
                                                                            "Columbia", "MO", "65203", "5738087408", "password", "password");
            
            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Patron with that email and password does not exist!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdatePatronByEmailTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod UpdatePatronByEmailTest throws exception");
            _mockPatronDao.Setup(check => check.CheckPatronCredentials(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(email => email.CheckEmailUnique(It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetPatronByEmail(It.IsAny<string>())).Returns(Task.FromResult(_patronModel));
            _mockPatronDao.Setup(patron => patron.UpdatePatronById(It.IsAny<PatronModel>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.UpdatePatronByEmail("Email", "patronPassword", "James", "Remus", "James.Remus@vu.com", "308 Devine Ct.", "Columbia", "MO", "65203", "5738087408", "password", "password");
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
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.DeletePatronById(It.IsAny<int>()));

            //Act
            var result = await _mockPatronsController.DeletePatronById(1, "password", _patronModel.Id);

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
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockPatronsController.DeletePatronById(1, "password", 5);

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
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.DeletePatronById(It.IsAny<int>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.DeletePatronById(1, "password", _patronModel.Id);
           
            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetPatronByIdTest_ExecutesQuery_ReturnsOkObjectResult()
        {
            //Arrange
            Console.WriteLine("Inside GetPatronByIdTest returns Ok result");
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.GetPatronById(_mockStaffModel.Id, _mockStaffModel.Password, _patronModel.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is OkObjectResult);
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
            Assert.AreEqual(_patronModel, (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetPatronByIdTest_ActionExecutes_ReturnsAdminErrorMessageWhenFalse()
        {
            //Arrange
            Console.WriteLine("Inside GetPatronIdTest returns admin error message.");
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModel));

            //Act
            var result = await _mockPatronsController.GetPatronById(_mockStaffModel.Id, _mockStaffModel.Password, _patronModel.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual(StatusCodes.Status403Forbidden, (result as ObjectResult).StatusCode);
            Assert.AreEqual("You need to have proper admin credentials to complete this task!", (result as ObjectResult).Value);
        }

        [TestMethod]
        public async Task GetPatronByIdTest_ExecutesQuery_ReturnsMessageWhenNull()
        {
            //Arrange
            Console.WriteLine("Inside GetPatronByIdTest returns null message.");
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Returns(Task.FromResult(_patronModelMockNull));

            //Act
            var result = await _mockPatronsController.GetPatronById(_mockStaffModel.Id, _mockStaffModel.Password, _patronModel.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Patron with that Id number does not exist!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetPatronByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetPatronByIdTest throws exception");
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockPatronDao.Setup(patron => patron.GetPatronById(It.IsAny<int>())).Throws<Exception>();

            //Act
            var result = await _mockPatronsController.GetPatronById(_mockStaffModel.Id, _mockStaffModel.Password, _patronModel.Id);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }
    }
}
