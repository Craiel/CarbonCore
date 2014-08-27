namespace CarbonCore.Tests.ContentServices
{
    using Autofac;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.IoC;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class DatabaseServiceTests
    {
        private IContainer container;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerBuilder.Build<ContentServicesModule>();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public void GeneralTests()
        {
            using (var service = this.container.Resolve<IDatabaseService>())
            {
                CarbonFile database = CarbonFile.GetTempFile();
                database.DeleteIfExists();
                service.Initialize(database);

                Assert.IsTrue(database.Exists, "Service initialize must create the database");
            }
        }

        [Test]
        public void InsertUpdateTests()
        {
            using (var service = this.container.Resolve<IDatabaseService>())
            {
                CarbonFile database = CarbonFile.GetTempFile();
                database.DeleteIfExists();
                service.Initialize(database);

                var clone = (ContentTestEntry)ContentTestData.TestEntry.Clone();

                service.Save(ref clone);
                Assert.NotNull(clone.Id, "Primary key value must be assigned on save");

                clone.TestString = "Updated value test";
                clone.TestFloat = 3.1f;
                service.Save(ref clone);

                var savedEntry = service.Load<ContentTestEntry>((int)clone.Id);
                Assert.AreEqual(clone, savedEntry);
            }
        }
    }
}
