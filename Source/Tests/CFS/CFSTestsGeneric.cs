namespace CarbonCore.Tests.CFS
{
    using CarbonCore.CFS.Logic;
    using CarbonCore.Utils.IO;

    using NUnit.Framework;

    [TestFixture]
    public class CFSTestsGeneric
    {
        [Test]
        public void MemoryTest()
        {
            var instance = new CFSInstanceMemory();
            Assert.AreEqual(0, instance.Files.Count);

            instance.Dispose();
        }

        [Test]
        public void FileTest()
        {
            CarbonFile testFile = CarbonFile.GetTempFile();
            testFile.DeleteIfExists();

            var instance = new CFSInstanceFile(testFile);
            Assert.AreEqual(0, instance.Files.Count);
            Assert.IsTrue(testFile.Exists);

            instance.Dispose();
        }
    }
}
