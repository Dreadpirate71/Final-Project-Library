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
        public void GetListOfAllPatronsTest_ActionExecutes_ReturnsOkWithData()
        {
            Console.WriteLine("Inside TestMethod GetListOfAllPatronsTest");
            var result = _mockPatronsController.GetListOfAllPatrons();
            Assert.IsNotNull(result);
        }
    }
}
