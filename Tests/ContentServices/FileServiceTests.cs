namespace CarbonCore.Tests.ContentServices
{
    using System;
    using System.Collections.Generic;
    using System.IO.Compression;
    using System.Linq;

    using CarbonCore.ContentServices.Compat.Logic;
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.IoC;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class FileServiceTests
    {
        private const string FileEntryResourcePath = "Resources.FileEntries";
        private const string FileEntryFolder = "FileEntries";

        private readonly IDictionary<FileEntryKey, CarbonFile> testFiles;

        private ICarbonContainer container;

        private CarbonDirectory dataDirectory;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileServiceTests()
        {
            this.testFiles = new Dictionary<FileEntryKey, CarbonFile>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerAutofacBuilder.Build<ContentServicesModule>();

            this.dataDirectory = CarbonDirectory.GetTempDirectory();

            IList<CarbonFile> files = this.GetType().Assembly.ExtractResources(this.dataDirectory, FileEntryResourcePath);
            Assert.AreEqual(4, files.Count, "Must extract all resources properly");

            // Build the dictionary of eligible test files
            this.testFiles.Clear();
            foreach (string file in Resources.Static.ResourceList)
            {
                if (!file.StartsWith(FileEntryFolder))
                {
                    continue;
                }

                string relativeFileName = file.Replace(FileEntryFolder + System.IO.Path.DirectorySeparatorChar, string.Empty);
                CarbonFile localFile = this.dataDirectory.ToFile(relativeFileName);
                Assert.IsTrue(localFile.Exists, "Test file must exist in the data directory!");

                this.testFiles.Add(new FileEntryKey(relativeFileName), localFile);
            }
        }

        [TearDown]
        public void TearDown()
        {
            // Cleanup after the test
            if (this.dataDirectory != null && this.dataDirectory.Exists)
            {
                this.dataDirectory.Delete(true);
            }
        }

        [Test]
        public void GeneralTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                Assert.NotNull(service, "Service must resolve properly");

                var memoryProvider = this.container.Resolve<IFileServiceMemoryProvider>();
                memoryProvider.Initialize();
                service.AddProvider(memoryProvider);

                var fileProvider = this.container.Resolve<IFileServiceDiskProvider>();
                fileProvider.Root = this.dataDirectory;
                fileProvider.Initialize();
                service.AddProvider(fileProvider);

                var packProvider = this.container.Resolve<IFileServicePackProvider>();
                packProvider.Root = this.dataDirectory;
                packProvider.Initialize();
                service.AddProvider(packProvider);

                Assert.AreEqual(3, service.GetProviders().Count, "Must have all providers registered");
                service.RemoveProvider(memoryProvider);
                service.RemoveProvider(fileProvider);
                service.RemoveProvider(packProvider);

                Assert.AreEqual(0, service.GetProviders().Count, "Must remove all providers");

                memoryProvider.Dispose();
                fileProvider.Dispose();
                packProvider.Dispose();
            }
        }

        [Test]
        public void MemoryProviderTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                using (var provider = this.container.Resolve<IFileServiceMemoryProvider>())
                {
                    provider.CompressionLevel = CompressionLevel.Optimal;
                    provider.Initialize();

                    service.AddProvider(provider);

                    this.PerformProviderTests(service, provider);

                    service.RemoveProvider(provider);
                    Assert.AreEqual(0, service.GetFileEntries().Count, "After removal service must have no files");
                }
            }
        }

        [Test]
        public void DiskProviderTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                using (var provider = this.container.Resolve<IFileServiceDiskProvider>())
                {
                    provider.CompressionLevel = CompressionLevel.Optimal;
                    provider.Root = this.dataDirectory;
                    provider.Initialize();

                    service.AddProvider(provider);

                    this.PerformProviderTests(service, provider);
                    
                    service.RemoveProvider(provider);
                    Assert.AreEqual(0, service.GetFileEntries().Count, "After removal service must have no files");
                }
            }
        }

        [Test]
        public void PackProviderTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                using (var provider = this.container.Resolve<IFileServicePackProvider>())
                {
                    provider.CompressionLevel = CompressionLevel.Optimal;
                    provider.Root = this.dataDirectory;
                    provider.Initialize();
                    service.AddProvider(provider);

                    this.PerformProviderTests(service, provider);

                    service.RemoveProvider(provider);
                    Assert.AreEqual(0, service.GetFileEntries().Count, "After removal service must have no files");
                }
            }
        }
        
        private void PerformProviderTests(IFileService service, IFileServiceProvider provider)
        {
            long testFileSize = 0;
            foreach (CarbonFile file in this.testFiles.Values)
            {
                testFileSize += file.Size;
            }

            // Test add
            foreach (FileEntryKey key in this.testFiles.Keys)
            {
                var data = new FileEntryData(this.testFiles[key].ReadAsByte());
                service.Save(key, data, provider);
            }

            Assert.AreEqual(testFileSize, provider.BytesWritten, "Must have written exact number of bytes");
            Assert.Greater(testFileSize, provider.BytesWrittenActual, "Must have exact number of actual bytes");
            Assert.AreEqual(0, provider.BytesRead, "Must have no bytes read yet");

            // Test load
            foreach (FileEntryKey key in this.testFiles.Keys)
            {
                var originalData = new FileEntryData(this.testFiles[key].ReadAsByte());
                FileEntryData loadedData = service.Load(key);
                Assert.AreEqual(originalData.ByteData, loadedData.ByteData, "Loaded data must match original");
            }

            Assert.AreEqual(testFileSize, provider.BytesRead, "Must have read exact number of bytes");
            Assert.Greater(testFileSize, provider.BytesReadActual, "Must have exact number of actual bytes");

            // Meta info get
            DateTime currentDate = DateTime.Now;
            FileEntryKey metaTestKey = this.testFiles.Keys.First();
            Assert.AreEqual(1, service.GetVersion(metaTestKey), "New files must have version 1 by default");
            Assert.AreEqual(currentDate.Date, service.GetCreateDate(metaTestKey).Date, "New added files must have current date as create date");
            Assert.AreEqual(currentDate.Date, service.GetModifiedDate(metaTestKey).Date, "New files must have current date as modified date");
            
            // Meta info set
            Assert.Throws<ArgumentException>(() => service.SetModifiedDate(metaTestKey, currentDate - TimeSpan.FromDays(1)), "Setting the modified date before the create date should throw");
            service.SetVersion(metaTestKey, 2);
            service.SetModifiedDate(metaTestKey, currentDate + TimeSpan.FromDays(1));
            service.SetCreateDate(metaTestKey, currentDate - TimeSpan.FromDays(1));
            Assert.AreNotEqual(service.GetModifiedDate(metaTestKey).Date, service.GetCreateDate(metaTestKey).Date, "Manually changing the modified date should make it un-equal");

            service.Update(metaTestKey, new FileEntryData(new byte[] { 20, 20, 30 }));
            Assert.AreEqual(3, service.GetVersion(metaTestKey), "Saving data must auto-increment the version");
            
            // Test remove
            foreach (FileEntryKey key in this.testFiles.Keys)
            {
                service.Delete(key);
            }

            Assert.AreEqual(this.testFiles.Count, provider.EntriesDeleted, "Must have deleted count of test files");

            Assert.AreEqual(this.testFiles.Count, service.Cleanup(), "Cleanup must purge the deleted entries");
        }
    }
}
