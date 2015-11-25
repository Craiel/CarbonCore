// ReSharper disable RedundantAssignment
namespace CarbonCore.Tests.Unity
{
    using CarbonCore.Tests.Unity.Data;
    using CarbonCore.Utils.Unity.Data;

    using NUnit.Framework;

    [TestFixture]
    public class ReferenceTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void WeakReferenceTest()
        {
            var testEntry = new ReferenceEntry();

            var reference = new WeakReference<ReferenceEntry>(testEntry);
            Assert.IsTrue(reference.IsAlive);
            Assert.IsNotNull(reference.Target);

            // Free the instance and force a collect
            testEntry = null;
            System.GC.Collect();

            // Now there reference should not be alive
            Assert.IsFalse(reference.IsAlive);
            Assert.IsNull(reference.Target);
        }

        [Test]
        public void RefCountedReferenceTest()
        {
            var testEntry = new ReferenceEntry();

            // Create 2 references
            var reference1 = new RefCountedWeakReference<ReferenceEntry>(testEntry);
            var reference2 = new RefCountedWeakReference<ReferenceEntry>(testEntry);

            // Both should be valid
            Assert.IsTrue(reference1.IsAlive);
            Assert.IsTrue(reference2.IsAlive);

            Assert.AreEqual(2, testEntry.RefCount());

            // Modify the original instance and check we get the same result in both references
            Assert.AreEqual(0, testEntry.IntValue);
            testEntry.IntValue = 1;
            Assert.AreEqual(1, reference1.Target.IntValue);
            Assert.AreEqual(1, reference2.Target.IntValue);

            // Release one instance and check the counter updates properly
            reference2.Dispose();
            reference2 = null;
            Assert.AreEqual(1, testEntry.RefCount());

            // Release the entry and check the alive status
            testEntry = null;
            System.GC.Collect();

            Assert.IsFalse(reference1.IsAlive);
        }
    }
}
