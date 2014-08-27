﻿namespace CarbonCore.Tests.Utils
{
    using CarbonCore.Utils.Database;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
        }

        [TearDown]
        public void TearDown()
        {
        }
        
        [Test]
        public void SqlStatementTests()
        {
            var testSelect = new SqlStatement(SqlStatementType.Select);
            testSelect.Table("TestTable");
            testSelect.What("Id");
            testSelect.What("TestString");
            testSelect.WhereConstraint("Id", 50);
            testSelect.WhereConstraint("TestString", "Super");
            testSelect.OrderBy("TestString");

            const string ControlSelect = @"SELECT Id,TestString FROM TestTable WHERE Id = @WHR_Id AND TestString = @WHR_TestString ORDER BY TestString";

            string testSelectStatement = testSelect.ToString();
            Assert.AreEqual(ControlSelect, testSelectStatement);
        }
    }
}
