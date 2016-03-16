namespace CarbonCore.Unity.Tests
{
    using CarbonCore.Unity.Tests.Data;

    using NUnit.Framework;

    [TestFixture]
    public class DataEntryTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void GeneralTests()
        {
            var testEntry = (DataTestEntry)DataTestData.TestEntry.Clone();
            testEntry.Id = 123;

            var clone = (DataTestEntry)testEntry.Clone();
            Assert.AreEqual(clone, testEntry, "Clone must equal original entry");

            var clone2 = (DataTestEntry)clone.Clone();
            Assert.AreEqual(clone, clone2, "Direct clone with a non-set id must match");

            clone2.TestByteArray = null;
            Assert.AreEqual(clone, clone2, "Direct clone must still match after equality ignore property set to null");

            clone2.TestString = "Another test clone";
            Assert.AreNotEqual(clone, clone2, "Clones must mis-match after changing data");
        }

        [Test]
        public void HashCodeTests()
        {
            var clone = (DataTestEntry)DataTestData.TestEntry.Clone();
            int originalHash = clone.GetHashCode();
            
            float originalFloat = clone.TestFloat;

            clone.TestFloat += 15.0f;
            Assert.AreNotEqual(originalHash, clone.GetHashCode(), "Hashes should mismatch after property change");

            clone.TestFloat = originalFloat;
            Assert.AreEqual(originalHash, clone.GetHashCode(), "Hash should revert to original");

            // Restore the data from the original
            clone.CopyFrom(DataTestData.TestEntry);
            Assert.AreEqual(clone.GetHashCode(), DataTestData.TestEntry.GetHashCode(), "Hashes of objects with the same properties should match");

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
    }
}
