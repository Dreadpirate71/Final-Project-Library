using LibraryAPI.Daos;
using LibraryAPI.Models;
using LibraryAPI.Wrappers;
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
    public class StaffDaoTests
    {
        private readonly Mock<ISqlWrapperStaff> _mockSqlWrapper;
        private readonly StaffDao _staffDaoMock;
        public StaffDaoTests()
        {
            _mockSqlWrapper = new Mock<ISqlWrapperStaff>();
            _staffDaoMock = new StaffDao(_mockSqlWrapper.Object);
        }
        [TestMethod]
        public void CallSqlWithSelectString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _staffDaoMock.GetListOfAllStaffTest();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryStaff<StaffModel>(It.Is<string>(sql => sql == "SELECT * FROM Patrons")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithUpdateString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _staffDaoMock.UpdateStaff();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryStaff<StaffModel>(It.Is<string>(sql => sql == "UPDATE Staff SET FirstName = @FirstName, LastName = @LastName," +
                                   $"PhoneNumber = @PhoneNumber, Position = @Position $WHERE Id = @Id")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithDeleteString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _staffDaoMock.DeleteStaff();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryStaff<StaffModel>(It.Is<string>(sql => sql == "DELETE FROM Staff WHERE Id = '{Id}'")), Times.Once);
        }



    }
}


