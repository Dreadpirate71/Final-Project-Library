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
    public class PatronDaoTests
    {
        private readonly Mock<ISqlWrapperPatron> _mockSqlWrapper;
        private readonly PatronDao _patronDaoMock;
        public PatronDaoTests()
        {
            _mockSqlWrapper = new Mock<ISqlWrapperPatron>();
            _patronDaoMock = new PatronDao(_mockSqlWrapper.Object);
        }
        [TestMethod]
        public void CallSqlWithSelectString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _patronDaoMock.GetListOfAllPatronsTest();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryPatron<PatronModel>(It.Is<string>(sql => sql == "SELECT * FROM Patrons")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithUpdateString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _patronDaoMock.UpdatePatron();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryPatron<PatronModel>(It.Is<string>(sql => sql == "UPDATE Patrons SET FirstName = @FirstName, LastName = @LastName," +
                                   $"Email = @Email, StreetAddress = @StreetAddress, City = @City, State = @State, PostalCode = @ PostalCode, PhoneNumber = @PhoneNumber" +
                                   $"WHERE Email = @Email")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithInsertString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _patronDaoMock.AddPatron();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryPatron<PatronModel>(It.Is<string>(sql => sql == $"INSERT INTO Patrons (FirstName, LastName, Email, StreetAddress, City, State, PostalCode, PhoneNumber)" +
                $"VALUES (@FirstName, @LastName, @Email, @StreetAddress, @PostalCode, @State, @PostalCode, @PhoneNumber)")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithUpdateWhereString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _patronDaoMock.GetPatronEmail();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryPatron<PatronModel>(It.Is<string>(sql => sql == "SELECT * FROM Patrons WHERE Email = '{Email}'")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithDeleteString_VerifyQueries_MatchingExpressionsConfirmed()
        {
            _patronDaoMock.DeletePatron();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryPatron<PatronModel>(It.Is<string>(sql => sql == "DELETE FROM Patrons WHERE Id = '{Id}'")), Times.Once);
        }
        [TestMethod]
        public void CallSqlWithSelectString_VerifyQueries_MatchingExpressonsConfirmed()
        {
            _patronDaoMock.GetPatronId();
            _mockSqlWrapper.Verify(sqlWrapper => sqlWrapper.QueryPatron<PatronModel>(It.Is<string>(sql => sql == "SELECT * FROM Patrons WHERE Id = '{Id}'")), Times.Once);
        }
    }
}
