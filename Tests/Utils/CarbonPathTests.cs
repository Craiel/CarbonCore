namespace CarbonCore.Tests.Compat.Utils
{
    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;

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

            var relative = current.GetParent().ToRelative<CarbonDirectory>(RuntimeInfo.WorkingDirectory);
            Assert.IsTrue(relative.Exists, "Relative directory must exist");
            Assert.IsTrue(relative.IsRelative, "Relative directory must be set to relative");

            string testPath = @"FirstPath/SecondPath\ThirdPath/ActualName\\";
            CarbonDirectory stringInitialized = new CarbonDirectory(testPath);
            Assert.AreEqual(testPath, stringInitialized.DirectoryName);
            Assert.AreEqual(@"ActualName", stringInitialized.DirectoryNameWithoutPath);
            Assert.NotNull(stringInitialized.GetParent().GetParent().GetParent());

            CarbonDirectory extended = stringInitialized.ToDirectory("OneMore");
            Assert.AreEqual("OneMore", extended.DirectoryNameWithoutPath);
            Assert.AreEqual(stringInitialized, extended.GetParent());

            long freeSpace = current.GetFreeSpace();
            Assert.Greater(freeSpace, 0, "GetFreeSpace must return non-zero value");

            CarbonDirectory parentTest = new CarbonDirectory(@"FirstPath//SamePath\\SamePath//SamePath\\Something_SamePath//EndPath");
            int iterations = 0;
            while (parentTest != null)
            {
                iterations++;
                parentTest = parentTest.GetParent();

                if (iterations >= 10)
                {
                    Assert.Fail("GetParent did not end in the expected duration, check implementation!");
                    break;
                }
            }

            Assert.AreEqual(5, iterations, "GetParent should have succeeded 5 times, got " + iterations);
        }

        [Test]
        public void SearchTests()
        {
            CarbonDirectory current = RuntimeInfo.WorkingDirectory;

            CarbonFileResult[] fileResults = current.GetFiles();
            Assert.Greater(fileResults.Length, 0);

            CarbonDirectoryResult[] dirResults = current.GetParent().GetDirectories();
            Assert.Greater(dirResults.Length, 0);
        }
    }
}