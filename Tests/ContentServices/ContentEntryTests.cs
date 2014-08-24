namespace CarbonCore.Tests.ContentServices
{
    using Autofac;

    using CarbonCore.ContentServices.IoC;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class ContentEntryTests
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
            ContentTestData.TestEntry.Id = 123;

            var clone = (ContentTestEntry)ContentTestData.TestEntry.Clone();
            Assert.AreNotEqual(clone, ContentTestData.TestEntry, "Clone must be different instance of original entry");
            Assert.IsNull(clone.Id, "Clone id must be null");

            var clone2 = (ContentTestEntry)clone.Clone();
            Assert.AreEqual(clone, clone2, "Direct clone with a non-set id must match");

            clone2.TestByteArray = null;
            Assert.AreEqual(clone, clone2, "Direct clone must still match after equality ignore property set to null");

            clone2.TestString = "Another test clone";
            Assert.AreNotEqual(clone, clone2, "Clones must mis-match after changing data");
        }
    }
}
