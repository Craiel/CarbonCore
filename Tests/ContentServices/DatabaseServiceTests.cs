namespace CarbonCore.Tests.ContentServices
{
    using Autofac;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.IoC;
    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseServiceTests
    {
        private const string TestDatabaseFile = "serviceTest.db";

        private static readonly CarbonFile TestFile = RuntimeInfo.WorkingDirectory.ToFile(TestDatabaseFile);
        
        private IContainer container;

        private IDatabaseService service;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerBuilder.Build<ContentServicesModule>();
            this.service = this.container.Resolve<IDatabaseService>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GeneralTests()
        {
            TestFile.DeleteIfExists();

            this.service.Initialize(TestFile);

            Assert.IsTrue(TestFile.Exists, "Service initialize must create the database");
        }

        [Test]
        public void InsertUpdateTests()
        {
            this.service.Initialize(TestFile);

            var clone = (ContentTestEntry)ContentTestData.TestEntry.Clone();

            this.service.Save(ref clone);
            Assert.NotNull(clone.Id, "Primary key value must be assigned on save");

            clone.TestString = "Updated value test";
            clone.TestFloat = 3.1f;
            this.service.Save(ref clone);

            var savedEntry = this.service.Load<ContentTestEntry>((int)clone.Id);
            Assert.AreEqual(clone, savedEntry);
        }
    }
}
