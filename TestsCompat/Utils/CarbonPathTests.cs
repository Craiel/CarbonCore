namespace CarbonCore.Tests.Compat.Utils
{
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.IO;

    using NUnit.Framework;

    [TestFixture]
    public class CarbonPathTests
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void DirectoryTests()
        {
            CarbonDirectory current = RuntimeInfo.WorkingDirectory;
            Assert.IsTrue(current.Exists, "Current directory has to exist");

            CarbonFileResult[] files = current.GetFiles();
            Assert.Greater(files.Length, 0, "GetFiles without arguments must return all in the same directory");

            long freeSpace = current.GetFreeSpace();
            Assert.Greater(freeSpace, 0, "GetFreeSpace must return non-zero value");

            var relative = current.GetParent().ToRelative<CarbonDirectory>(RuntimeInfo.WorkingDirectory);
            Assert.IsTrue(relative.Exists, "Relative directory must exist");
            Assert.IsTrue(relative.IsRelative, "Relative directory must be set to relative");
        }
    }
}