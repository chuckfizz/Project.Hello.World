using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hello.World.UnitTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethodOracle()
        {
            // arrange
            OracleDataAccess db = new OracleDataAccess();

            // act
            string data = db.GetSomeData();

            // assert is handled by ExpectedException
            Assert.AreEqual(data, "Data will come from a Oracle Database");
        }
        [TestMethod]
        public void TestMethodSqlServer()
        {
            // arrange
            SqlDataAccess db = new SqlDataAccess();

            // act
            string data = db.GetSomeData();

            // assert is handled by ExpectedException
            Assert.AreEqual(data, "Data will come from a Sql Database");
        }
        [TestMethod]
        public void TestMethodRestService()
        {
            // arrange
            WebServiceAccess db = new WebServiceAccess();

            // act
            string data = db.GetSomeData();

            // assert is handled by ExpectedException
            Assert.AreEqual(data, "Data will come from a Web Service");
        }

    }
}
