namespace CarbonCore.Tests.ContentServices
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    using Autofac;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.IoC;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.Compat.IO;
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
            this.container = new CarbonContainerAutofacBuilder().Build<ContentServicesModule>() as IContainer;
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
                this.TestGeneric(service);
            }
        }
        
        [Test]
        public void ServiceTests()
        {
            using (var service = this.container.Resolve<IDatabaseService>())
            {
                this.TestServiceDetails(service);
            }
        }

        [Test]
        public void ServiceTestsAsync()
        {
            using (var service = this.container.Resolve<IDatabaseService>())
            {
                CarbonFile database = CarbonFile.GetTempFile();
                database.DeleteIfExists();
                service.Initialize(database);

                // Batch save
                IList<ContentTestEntry2> batchData = new List<ContentTestEntry2>();
                for (var i = 0; i < 20; i++)
                {
                    var clone2 = (ContentTestEntry2)ContentTestData.TestEntry2.Clone();
                    clone2.Id = "BE" + i;
                    clone2.OtherTestFloat += i;
                    clone2.OtherTestString = "Batch entry " + i.ToString(CultureInfo.InvariantCulture);
                    batchData.Add(clone2);
                }

                service.Save(batchData, async: true);
                service.Delete<ContentTestEntry2>(new List<object> { "BE5", "BE7", "BE12" }, async: true);

                batchData.Clear();
                for (var i = 0; i < 20; i++)
                {
                    var clone2 = (ContentTestEntry2)ContentTestData.TestEntry2.Clone();
                    clone2.Id = "CE" + i;
                    clone2.OtherTestFloat += i;
                    clone2.OtherTestString = "Batch entry " + i.ToString(CultureInfo.InvariantCulture);
                    batchData.Add(clone2);
                }

                service.Save(batchData, async: true);
                service.Delete<ContentTestEntry2>(new List<object> { "CE5", "CE7", "CE12" }, async: true);

                service.WaitForAsyncActions();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void TestGeneric(IDatabaseService service)
        {
            CarbonFile database = CarbonFile.GetTempFile();
            database.DeleteIfExists();
            service.Initialize(database);

            Assert.IsTrue(database.Exists, "Service initialize must create the database");
        }

        private void TestServiceDetails(IDatabaseService service)
        {
            CarbonFile database = CarbonFile.GetTempFile();
            database.DeleteIfExists();
            service.Initialize(database);

            // Save
            var clone = (ContentTestEntry)ContentTestData.TestEntry.Clone();
            service.Save(ref clone);
            Assert.NotNull(clone.Id, "Primary key value must be assigned on save");

            // Save 5 more test entries
            for (int i = 0; i < 5; i++)
            {
                clone.Id = null;
                clone.TestFloat++;
                service.Save(ref clone);
            }

            // Joined save
            clone.JoinedEntry = new ContentTestJoinedEntry { TestString = "I am joined with " + clone.Id };
            service.Save(ref clone);
            Assert.IsNotNull(clone.JoinedEntry.Id, "Joined entry must be saved together with the parent");
            Assert.AreEqual(clone.Id, clone.JoinedEntry.TestEntryId, "Joined entry foreign key value must match the original");

            Assert.AreEqual(service.Load<ContentTestJoinedEntry>().Count, 1, "After saving main instance we must have 1 joined entry");
            clone.JoinedEntry = null;
            service.Save(ref clone);
            Assert.AreEqual(service.Load<ContentTestJoinedEntry>().Count, 0, "After saving main instance with null joined instance the instance must be deleted");

            clone.JoinedEntry = new ContentTestJoinedEntry { TestString = "Yet another joined entry of " + clone.Id };
            service.Save(ref clone);

            // Load
            var savedEntry = service.Load<ContentTestEntry>((int)clone.Id);
            Assert.AreEqual(clone, savedEntry, "Load must return the proper entry");
            Assert.IsNull(savedEntry.JoinedEntry, "Joined entry must not be loaded with the original on default load");

            // Load full
            savedEntry = service.Load<ContentTestEntry>((int)clone.Id, true);
            Assert.IsNotNull(savedEntry.JoinedEntry, "Joined entry must be loaded with the original when loading fully");

            // Load batch
            IList<ContentTestEntry> batchResult = service.Load<ContentTestEntry>();
            Assert.AreEqual(6, batchResult.Count, "Load with no parameters must return all entries");
            
            // Delete
            service.Delete<ContentTestEntry>(1);
            savedEntry = service.Load<ContentTestEntry>(1);
            Assert.IsNull(savedEntry, "Load after delete should return no entry");

            // GetTables
            IList<string> tables = service.GetTables();
            Assert.AreEqual(2, tables.Count, "GetTables must return the table list");
            Assert.IsTrue(tables.Contains("TestTable"));

            // Drop
            service.Drop<ContentTestEntry>();
            tables = service.GetTables();
            Assert.AreEqual(1, tables.Count, "Drop must remove the table");
            Assert.Throws<InvalidOperationException>(service.Drop<ContentTestEntry>, "Explicit Drop must fail if table does not exist");
        }
    }
}
