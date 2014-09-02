namespace CarbonCore.Tests.ContentServices
{
    using System.IO;

    using NUnit.Framework;

    [TestFixture]
    public class ContentEntryTests
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
        
        [Test]
        public void SaveLoadTests()
        {
            var clone = (ContentTestEntry)ContentTestData.TestEntry.Clone();
            using (var stream = new MemoryStream())
            {
                Assert.IsFalse(ContentTestData.TestEntry2.Save(stream), "Entry with no save implementation should fail saving");

                clone.Save(stream);
                Assert.Greater(stream.Position, 0, "Entry should write itself to the stream");

                stream.Seek(0, SeekOrigin.Begin);
                var loadTest = new ContentTestEntry();
                loadTest.Load(stream);

                Assert.AreEqual(clone, loadTest, "Loading should result in an equal state");
            }
        }
    }
}
