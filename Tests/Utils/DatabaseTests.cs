namespace CarbonCore.Tests.Utils
{
    using CarbonCore.Utils.Compat.Database;
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
            testSelect.WhereConstraint(new SqlStatementConstraint("Id", 50));
            testSelect.WhereConstraint(new SqlStatementConstraint("TestString", "Super"));
            testSelect.OrderBy(new SqlStatementOrderByRule("TestString"));

            const string ControlSelect = @"SELECT Id,TestString FROM TestTable WHERE Id = @WHR_Id AND TestString = @WHR_TestString ORDER BY TestString DESC";

            string testSelectStatement = testSelect.ToString();
            Assert.AreEqual(ControlSelect, testSelectStatement);
        }
    }
}
