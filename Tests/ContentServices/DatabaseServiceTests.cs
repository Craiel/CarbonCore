namespace CarbonCore.Tests.ContentServices
{
    using System.Collections.Generic;
    using System.Globalization;

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
        public void ServiceTests()
        {
            using (var service = this.container.Resolve<IDatabaseService>())
            {
                CarbonFile database = CarbonFile.GetTempFile();
                database.DeleteIfExists();
                service.Initialize(database);

                // Save
                var clone = (ContentTestEntry)ContentTestData.TestEntry.Clone();
                
                Assert.IsTrue(service.Save(ref clone));
                Assert.NotNull(clone.Id, "Primary key value must be assigned on save");

                clone.TestString = "Updated value test";
                clone.TestFloat = 3.1f;
                Assert.IsTrue(service.Save(ref clone));

                // Joined save
                clone.JoinedEntry = new ContentTestJoinedEntry { TestString = "I am joined with " + clone.Id };
                Assert.IsTrue(service.Save(ref clone));
                Assert.IsNotNull(clone.JoinedEntry.Id, "Joined entry must be saved together with the parent");
                Assert.AreEqual(clone.Id, clone.JoinedEntry.TestEntryId, "Joined entry foreign key value must match the original");

                Assert.AreEqual(service.Load<ContentTestJoinedEntry>().Count, 1, "After saving main instance we must have 1 joined entry");
                clone.JoinedEntry = null;
                Assert.IsTrue(service.Save(ref clone));
                Assert.AreEqual(service.Load<ContentTestJoinedEntry>().Count, 0, "After saving main instance with null joined instance the instance must be deleted");

                clone.JoinedEntry = new ContentTestJoinedEntry { TestString = "Yet another joined entry of " + clone.Id };
                Assert.IsTrue(service.Save(ref clone));

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

                Assert.IsTrue(service.Save(batchData));
                
                // Load
                var savedEntry = service.Load<ContentTestEntry>((int)clone.Id);
                Assert.AreEqual(clone, savedEntry, "Load must return the proper entry");
                Assert.IsNull(savedEntry.JoinedEntry, "Joined entry must not be loaded with the original on default load");

                // Load full
                savedEntry = service.Load<ContentTestEntry>((int)clone.Id, true);
                Assert.IsNotNull(savedEntry.JoinedEntry, "Joined entry must be loaded with the original when loading fully");

                // Load batch
                IList<ContentTestEntry2> batchResult = service.Load<ContentTestEntry2>();
                Assert.AreEqual(20, batchResult.Count, "Load with no parameters must return all entries");

                batchResult = service.Load<ContentTestEntry2>(new List<object> { "BE5", "BE10", "BE15" });
                Assert.AreEqual(3, batchResult.Count, "Load with parameters must return exact count");

                // Delete
                Assert.IsTrue(service.Delete<ContentTestEntry>(new List<object> { (int)clone.Id }));
                savedEntry = service.Load<ContentTestEntry>((int)clone.Id);
                Assert.IsNull(savedEntry, "Load after delete should return no entry");

                IList<object> batchDelete = new List<object>();
                for (var i = 0; i < 10; i++)
                {
                    batchDelete.Add(string.Format("BE{0}", i + 1));
                }

                Assert.IsTrue(service.Delete<ContentTestEntry2>(batchDelete));
                batchResult = service.Load<ContentTestEntry2>();
                Assert.AreEqual(10, batchResult.Count, "Load after delete must return less entries");

                // GetTables
                IList<string> tables = service.GetTables();
                Assert.AreEqual(4, tables.Count, "GetTables must return the table list");
                Assert.IsTrue(tables.Contains("TestTable"));
                Assert.IsTrue(tables.Contains("TestTable2"));

                // Drop
                Assert.IsTrue(service.Drop<ContentTestEntry>());
                tables = service.GetTables();
                Assert.AreEqual(3, tables.Count, "Drop must remove the table");
                Assert.IsFalse(service.Drop<ContentTestEntry>(), "Explicit Drop must fail if table does not exist");

                service.Drop<ContentTestEntry2>();
                tables = service.GetTables();
                Assert.AreEqual(2, tables.Count, "Drop must remove the table");
            }
        }
    }
}
