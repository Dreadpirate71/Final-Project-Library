using LibraryAPI.Controllers;
using LibraryAPI.Daos;
using LibraryAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryApi.UnitTest
{
    [TestClass]
    public class StaffControllerTests
    {
        private readonly Mock<IStaffDao> _mockStaffDao;
        private readonly StaffController _mockStaffController;
        private readonly StaffModel _mockStaffModel;
        private IEnumerable<StaffModel> _staff;
        private StaffModel? _mockStaffModelNull;
        public StaffControllerTests() 
        { 
            _mockStaffDao = new Mock<IStaffDao>();
            _mockStaffController = new StaffController (_mockStaffDao.Object);
            _mockStaffModel = new StaffModel() { Id = 22, FirstName = "Kris", LastName = "Remus", PhoneNumber = "5738086263", Position = "Librarian" };
            _staff = new List<StaffModel>() { _mockStaffModel };

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
        public async Task AddStaffTest_SubmitFields_ReturnsOkMessage()
        {
            //Arrange
            Console.WriteLine("Inside AddStaffTest returns OK message.");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockStaffDao.Setup(staff => staff.AddStaff("Kris", "Remus", "5738086263", "Librarian", "password"));

            //Act
            var result = await _mockStaffController.AddStaff(1, "password", "Kris", "Remus", "5738086263", "Librarian", "password", "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Staff member has been added.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task AddStaffTest_QueryExecutes_ReturnsMessageWhenCheckAdminIsFalse()
        {
            //Arrange
            Console.WriteLine("Inside GetStaff test returns admin error message.");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            //Act
            var result = await _mockStaffController.AddStaff(1, "password", "Kris", "Remus", "5738086263", "Librarian", "password", "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("You need to have proper admin credentials to complete this task!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task AddStaffTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod AddPatron throws exception");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockStaffDao.Setup(staff => staff.AddStaff("Kris", "Remus", "5738086263", "Librarian", "password")).Throws<Exception>();

            //Act
            var result = await _mockStaffController.AddStaff(1, "password", "Kris", "Remus", "5738086263", "Librarian", "password", "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetStaffTest_QueryExecutes_ReturnsOkObjectResult()
        {
            //Arrange
            Console.WriteLine("Inside GetStaffTest returns OK message with object");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockStaffDao.Setup(staff => staff.GetStaff()).Returns(Task.FromResult(_staff));

            //Act
            var result = await _mockStaffController.GetStaff(1, "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual(StatusCodes.Status200OK, (result as OkObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetStaffTest_QueryExecutes_ReturnsMessageWhenCheckAdminIsFalse()
        {
            //Arrange
            Console.WriteLine("Inside GetStaff test returns admin error message.");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            //Act
            var result = await _mockStaffController.GetStaff(1, "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("You need to have proper admin credentials to complete this task!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task GetStaffTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetStaff throws exception");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockStaffDao.Setup(staff => staff.GetStaff()).Throws<Exception>();

            //Act
            var result = await _mockStaffController.GetStaff(1, "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeleteStaffByIdTest_TaskExecutes_Returns200MessageWhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside DeleteStaffById test returns 200.");
            _mockStaffDao.Setup(staff => staff.GetStaffById(It.IsAny<int>())).Returns(Task.FromResult(_mockStaffModel));
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockStaffController.DeleteStaffById(1, "password", 3);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Staff member has been deleted.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeleteStaffByIdTest_TaskExecutes_ReturnsMessageWhenStaffIsNull()
        {
            //Arrange
            Console.WriteLine("Inside DeleteStaffById test returns null message.");
            _mockStaffDao.Setup(staff => staff.GetStaffById(It.IsAny<int>())).Returns(Task.FromResult(_mockStaffModelNull));
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockStaffController.DeleteStaffById(1, "password", 3);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Staff member with that Id does not exist!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeleteStaffByIdTest_TaskExecutes_ReturnsMessageWhenCheckAdminIsFalse()
        {
            //Arrange
            Console.WriteLine("Inside DeleteStaffById test returns null message.");
            _mockStaffDao.Setup(staff => staff.GetStaffById(It.IsAny<int>())).Returns(Task.FromResult(_mockStaffModel));
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(false));
            //Act
            var result = await _mockStaffController.DeleteStaffById(1, "password", 3);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("You need to have proper admin credentials to complete this task!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task DeleteStaffByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetStaff throws exception");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockStaffDao.Setup(staff => staff.GetStaffById(It.IsAny<int>())).Returns(Task.FromResult(_mockStaffModel));
            _mockStaffDao.Setup(staff => staff.DeleteStaffById(It.IsAny<int>())).Throws<Exception>();

            //Act
            var result = await _mockStaffController.DeleteStaffById(1, "password", 3);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateStaffByIdTest_UpdateRecord_ReturnsCode200WhenSuccessful()
        {
            //Arrange
            Console.WriteLine("Inside UpdateStaffTest returns code 200.");
            _mockStaffDao.Setup(staff => staff.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockStaffDao.Setup(staff => staff.GetStaffById(It.IsAny<int>())).Returns(Task.FromResult(_mockStaffModel));
            _mockStaffDao.Setup(staff => staff.UpdateStaffById(It.IsAny<StaffModel>())).Returns(Task.FromResult(_mockStaffModel));

            //Act
            var result = await _mockStaffController.UpdateStaffById(1, "password", 3, "Kris", "Remus", "5738086263", "Librarian", "password", "password");

            //Assert
            Console.WriteLine(result);
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Staff member has been updated.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status200OK, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateStaffByIdTest_TaskExecutes_ReturnsMessageWhenGetStaffIsNull()
        {
            //Arrange
            Console.WriteLine("Inside UpdateStaffById test returns null message.");
            _mockStaffDao.Setup(staff => staff.GetStaffById(It.IsAny<int>())).Returns(Task.FromResult(_mockStaffModelNull));
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            //Act
            var result = await _mockStaffController.UpdateStaffById(1, "password", 3, "Kris", "Remus", "5738086263", "Librarian", "password", "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("Staff member with that Id does not exist!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status404NotFound, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateStaffByIdTest_TaskExecutes_ReturnsMessageWhenCheckAdminIsFalse()
        {
            //Arrange
            Console.WriteLine("Inside UpdateStaffById test returns null message.");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            //Act
            var result = await _mockStaffController.UpdateStaffById(1, "password", 3, "Kris", "Remus", "5738086263", "Librarian", "password", "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(ObjectResult));
            Assert.AreEqual("You need to have proper admin credentials to complete this task!", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status400BadRequest, (result as ObjectResult).StatusCode);
        }

        [TestMethod]
        public async Task UpdateStaffByIdTest_ThrowsException_ReturnsExceptionError()
        {
            //Arrange
            Console.WriteLine("Inside TestMethod GetStaff throws exception");
            _mockStaffDao.Setup(staffAdmin => staffAdmin.CheckStaffForAdmin(It.IsAny<int>(), It.IsAny<string>())).Returns(Task.FromResult(true));
            _mockStaffDao.Setup(staff => staff.GetStaffById(It.IsAny<int>())).Returns(Task.FromResult(_mockStaffModel));
            _mockStaffDao.Setup(staff => staff.UpdateStaffById(It.IsAny<StaffModel>())).Throws<Exception>();
            
            //Act
            var result = await _mockStaffController.UpdateStaffById(1, "password", 3, "Kris", "Remus", "5738086263", "Librarian", "password", "password");

            //Assert
            Assert.IsNotNull(result);
            Assert.IsTrue(result is ObjectResult);
            Assert.AreEqual("Exception of type 'System.Exception' was thrown.", (result as ObjectResult).Value);
            Assert.AreEqual(StatusCodes.Status500InternalServerError, (result as ObjectResult).StatusCode);
        }
    }
}
