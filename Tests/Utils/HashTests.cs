namespace CarbonCore.Tests.Compat.Utils
{
    using CarbonCore.Utils;

    using NUnit.Framework;

    [TestFixture]
    public class HashTests
    {
        [Test]
        public void TestSimpleCombine()
        {
            var firstSet = new[] { 0, 1, 0 };
            int first = HashUtils.GetSimpleCombinedHashCode(firstSet);
            int second = HashUtils.GetSimpleCombinedHashCode(firstSet);
            int third = HashUtils.GetSimpleCombinedHashCode(firstSet);
            Assert.AreEqual(first, second);
            Assert.AreEqual(second, third);

            int mismatch1 = HashUtils.GetSimpleCombinedHashCode(new[] { 0, 1, 1 });
            Assert.AreNotEqual(first, mismatch1);
        }
    }
}
