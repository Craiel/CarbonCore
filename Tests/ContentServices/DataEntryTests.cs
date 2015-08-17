namespace CarbonCore.Tests.ContentServices
{
    using System.IO;

    using CarbonCore.ContentServices.Logic;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    using NUnit.Framework;

    [TestFixture]
    public class DataEntryTests
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
            DataTestData.TestEntry.Id = 123;

            var clone = (DataTestEntry)DataTestData.TestEntry.Clone();
            Assert.AreNotEqual(clone, DataTestData.TestEntry, "Clone must be different instance of original entry");
            Assert.IsNull(clone.Id, "Clone id must be null");

            var clone2 = (DataTestEntry)clone.Clone();
            Assert.AreEqual(clone, clone2, "Direct clone with a non-set id must match");

            clone2.TestByteArray = null;
            Assert.AreEqual(clone, clone2, "Direct clone must still match after equality ignore property set to null");

            clone2.TestString = "Another test clone";
            Assert.AreNotEqual(clone, clone2, "Clones must mis-match after changing data");
        }
        
        [Test]
        public void SaveLoadTests()
        {
            var clone = (DataTestEntry)DataTestData.TestEntry.Clone();
            using (var stream = new MemoryStream())
            {
                Assert.IsFalse(DataTestData.TestEntry2.Save(stream), "Entry with no save implementation should fail saving");

                clone.Save(stream);
                Assert.Greater(stream.Position, 0, "Entry should write itself to the stream");

                stream.Seek(0, SeekOrigin.Begin);
                var loadTest = new DataTestEntry();
                loadTest.Load(stream);

                Assert.AreEqual(clone, loadTest, "Loading should result in an equal state");
            }
        }

        [Test]
        public void HashCodeTests()
        {
            // Custom GetHashCode()
            var customHashCode = (DataTestEntry)DataTestData.TestEntry.Clone();
            int originalHash = customHashCode.GetHashCode();

            customHashCode.TestFloat += 25.0f;
            Assert.AreEqual(originalHash, customHashCode.GetHashCode(), "Custom hash override should stay consistent even with property change");

            // Default GetHashCode
            var clone = (DataTestEntry2)DataTestData.TestEntry2.Clone();
            originalHash = clone.GetHashCode();

            float originalFloat = clone.OtherTestFloat;

            clone.OtherTestFloat += 15.0f;
            Assert.AreNotEqual(originalHash, clone.GetHashCode(), "Hashes should mismatch after property change");

            clone.OtherTestFloat = originalFloat;
            Assert.AreEqual(originalHash, clone.GetHashCode(), "Hash should revert to original");

            // Restore the data from the original
            clone.CopyFrom(DataTestData.TestEntry2);
            Assert.AreEqual(clone.GetHashCode(), DataTestData.TestEntry2.GetHashCode(), "Hashes of objects with the same properties should match");

            var firstEntry = new DataTestEntry2();
            var secondEntry = new DataTestEntry2();
            Assert.AreEqual(firstEntry.GetHashCode(), secondEntry.GetHashCode());
            Assert.AreEqual(firstEntry, secondEntry);

            // DataTestEntry3 is configured to use the original GetHashCode function even though
            //  it inherites from DataTestEntry2
            var thirdEntry = new DataTestEntry3();
            Assert.AreNotEqual(firstEntry.GetHashCode(), thirdEntry.GetHashCode());
            Assert.AreNotEqual(firstEntry, thirdEntry);
        }

        [Test]
        public void SerializationTests()
        {
            var clone = (DataTestEntry)DataTestData.TestEntry.Clone();

            // Test basic Json serialization
            byte[] jsonData = DataEntrySerialization.Save(clone);
            Assert.AreEqual(131, jsonData.Length, "Serialization should return data");

            DataTestEntry restored = DataEntrySerialization.Load<DataTestEntry>(jsonData);
            Assert.AreEqual(clone, restored, "Deserialized data should match original class");

            // Test compact serialization
            byte[] compact = DataEntrySerialization.CompactSave(clone);
            Assert.AreEqual(5, compact.Length);
            Assert.Less(compact.Length, jsonData.Length, "Compact serialization should be smaller");

            DataTestEntry restoredCompact = DataEntry.CompactLoad<DataTestEntry>(compact);
            Assert.AreEqual(clone, restoredCompact, "Compact Deserialization should match source data");
        }
    }
}
