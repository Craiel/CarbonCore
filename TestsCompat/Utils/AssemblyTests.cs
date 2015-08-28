namespace CarbonCore.Tests.Compat.Utils
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Tests.Resources;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.IO;

    using NUnit.Framework;

    [TestFixture]
    public class AssemblyTests
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
        public void RuntimeInfoTests()
        {
            Assert.NotNull(RuntimeInfo.Assembly);
            Assert.NotNull(RuntimeInfo.Path);
            Assert.NotNull(RuntimeInfo.ProcessId);
            Assert.NotNull(RuntimeInfo.ProcessName);

            Assert.IsTrue(string.Equals("CarbonCore.Utils.Compat.dll", RuntimeInfo.AssemblyName, StringComparison.OrdinalIgnoreCase), "In test environment we should be at utils runtime info");
        }

        [Test]
        public void GeneralAssemblyTests()
        {
            Assert.IsTrue(UnitTest.IsRunningFromNunit, "UnitTest environment bust be recognized");

            Assert.NotNull(AssemblyExtensions.GetVersion(RuntimeInfo.Assembly.GetType()));

            // Check if the extensions are setup properly to get the directory and file info
            CarbonDirectory directory = RuntimeInfo.Assembly.GetDirectory();
            Assert.NotNull(directory);
            Assert.IsTrue(directory.Exists);

            CarbonFile file = RuntimeInfo.Assembly.GetAssemblyFileCompat();
            Assert.NotNull(file);
            Assert.IsTrue(file.Exists);
            
            IList<CarbonFile> assemblies = AssemblyExtensions.GetLoadedAssemblyFiles();
            Assert.NotNull(assemblies);
            Assert.GreaterOrEqual(assemblies.Count, 1);
        }

        [Test]
        public void ResourceExtractionTests()
        {
            CarbonDirectory testDirectory = CarbonDirectory.GetTempDirectory();

            IList<CarbonFile> resources = this.GetType().Assembly.ExtractResources(testDirectory, "TESTBLA");
            Assert.IsNull(resources, "Giving wrong path must extract none");

            resources = this.GetType().Assembly.ExtractResources(testDirectory);
            Assert.AreEqual(5, resources.Count, "Giving no path must extract all");
            
            resources = this.GetType().Assembly.ExtractResources(testDirectory, "Resources.FileEntries");
            Assert.AreEqual(4, resources.Count, "Giving path must extract partial resources");

            const string FileEntryTestRoot = "FileEntries";
            foreach (string resource in Static.ResourceList)
            {
                if (!resource.StartsWith(FileEntryTestRoot))
                {
                    continue;
                }

                string testFileName = resource.Replace(FileEntryTestRoot + @"\", string.Empty);
                CarbonFile resourceFile = testDirectory.ToFile(testFileName);
                Assert.IsTrue(resourceFile.Exists, "Resource must exist after extraction: " + resource);
            }

            testDirectory.Delete(true);
        }
    }
}
