namespace CarbonCore.Tests.Utils
{
    using System.Text.RegularExpressions;

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
            CarbonDirectory current = RuntimeInfo.SystemDirectory;
            Assert.IsTrue(current.Exists, "Current directory has to exist");
            
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
            Assert.IsTrue(stringInitialized.GetPath().StartsWith(extended.GetParent().GetPath()));

            long freeSpace = extended.GetFreeSpace();
            Assert.AreEqual(0, freeSpace, "GetFreeSpace should return 0 for relative paths");

            CarbonDirectory absolute = current.ToAbsolute<CarbonDirectory>();
            freeSpace = absolute.GetFreeSpace();
            Assert.Greater(freeSpace, 0, "GetFreeSpace must return non-zero value for absolute paths");
        }

        [Test]
        public void ParentTests()
        {
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

            Assert.AreEqual(6, iterations, "GetParent should have succeeded 5 times, got " + iterations);

            parentTest = new CarbonDirectory(@"D:\Directory (5)\With spaces and (brackets)\");
            var relative = parentTest.ToRelative<CarbonDirectory>(parentTest.GetParent());
            Assert.NotNull(relative);
        }

        [Test]
        public void FindParentTests()
        {
            CarbonDirectory firstTestPath = new CarbonDirectory(@"C:\SomeDirectory\AnotherDirectory\Third\Fourth");
            CarbonDirectory secondTestPath = new CarbonDirectory(@"C:/SomeDirectory/AnotherDirectory/Third/Fourth");
            CarbonDirectory thirdTestPath = new CarbonDirectory(@"/SomeDirectory/AnotherDirectory/Third/Fourth/Fourth/Fourth");

            CarbonDirectory result = firstTestPath.FindParent("AnotherDirectory");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsNull);

            result = firstTestPath.FindParent("Another", false);
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsNull);

            result = firstTestPath.FindParent("another", false, true);
            Assert.IsNull(result);

            result = secondTestPath.FindParent("AnotherDirectory");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsNull);

            result = thirdTestPath.FindParent("Fourth");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsNull);

            result = result.FindParent("Fourth");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsNull);

            result = thirdTestPath.FindParent("AnotherDirectory");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.IsNull);
        }

        [Test]
        public void SearchTests()
        {
            CarbonDirectory current = RuntimeInfo.SystemDirectory;

            CarbonFileResult[] fileResults = current.GetFiles();
            Assert.Greater(fileResults.Length, 0, "GetFiles without arguments must return all in the same directory: " + current);

            CarbonDirectoryResult[] dirResults = current.GetParent().GetDirectories();
            Assert.Greater(dirResults.Length, 0, "GetDirectories without arguments must return all in the same directory: " + current.GetParent());
        }
    }
}