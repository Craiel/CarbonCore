namespace CarbonCore.Tests.Utils
{
    using System.Collections.Generic;

    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;

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

            Assert.AreEqual("CarbonCore.Utils.dll", RuntimeInfo.AssemblyName, "In test environment we should be at utils runtime info");
        }

        [Test]
        public void GeneralAssemblyTests()
        {
            Assert.IsTrue(UnitTest.IsRunningFromNunit, "UnitTest environment bust be recognized");

            Assert.NotNull(RuntimeInfo.Assembly.GetVersion());

            // Check if the extensions are setup properly to get the directory and file info
            CarbonDirectory directory = RuntimeInfo.Assembly.GetDirectory();
            Assert.NotNull(directory);
            Assert.IsTrue(directory.Exists);

            CarbonFile file = RuntimeInfo.Assembly.GetAssemblyFile();
            Assert.NotNull(file);
            Assert.IsTrue(file.Exists);

            IList<CarbonFile> assemblies = AssemblyExtensions.GetLoadedAssemblyFiles();
            Assert.NotNull(assemblies);
            Assert.GreaterOrEqual(assemblies.Count, 1);
        }
    }
}
