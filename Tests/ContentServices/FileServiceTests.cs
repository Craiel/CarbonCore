namespace CarbonCore.Tests.ContentServices
{
    using System;
    using System.Collections.Generic;

    using Autofac;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.IoC;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.IoC;

    using NUnit.Framework;

    [TestFixture]
    public class FileServiceTests
    {
        private const string FileEntryResourcePath = "Resources.FileEntries";
        private const string FileEntryFolder = "FileEntries";

        private IContainer container;

        private CarbonDirectory dataDirectory;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SetUp]
        public void Setup()
        {
            this.container = CarbonContainerBuilder.Build<ContentServicesModule>();

            this.dataDirectory = CarbonDirectory.GetTempDirectory();

            IList<CarbonFile> files = this.GetType().Assembly.ExtractResources(this.dataDirectory, FileEntryResourcePath);
            Assert.AreEqual(4, files.Count, "Must extract all resources properly");
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

                service.AddProvider(this.container.Resolve<IFileServiceMemoryProvider>());
                service.AddProvider(this.container.Resolve<IFileServiceDiskProvider>());
                service.AddProvider(this.container.Resolve<IFileServicePackProvider>());

                Assert.AreEqual(4, service.GetProviders().Count, "Must have all three providers registered");
            }
        }

        [Test]
        public void MemoryProviderTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                using (var provider = this.container.Resolve<IFileServiceMemoryProvider>())
                {
                    provider.Initialize();

                    service.AddProvider(provider);

                    Assert.True(false, "Todo");

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
                    provider.Root = this.dataDirectory;
                    provider.Initialize();

                    service.AddProvider(provider);

                    foreach (string file in Resources.Static.ResourceList)
                    {
                        if (!file.StartsWith(FileEntryFolder))
                        {
                            continue;
                        }

                        string localFile = file.Replace(FileEntryFolder + @"\", string.Empty);
                        var testFile = this.dataDirectory.ToFile(localFile);
                        IFileEntry entry = provider.CreateEntry(testFile);
                        IFileEntryData entryData = new FileEntryData { Data = testFile.ReadAsByte() };

                        Assert.Throws<ArgumentException>(() => service.Save(entry, entryData), "Saving hashed entry without filename should throw");
                        service.Save(entry, entryData, localFile);

                        Assert.NotNull(entry.Hash, "Hash needs to be set after call to save");

                        // Now try to save it again this should work fine
                        service.Save(entry, entryData);

                        Assert.Throws<ArgumentException>(() => service.Save(entry, entryData, "TestMe"), "Saving hashed entry with a filename should throw");
                    }

                    Assert.AreEqual(4, service.GetFileEntries().Count, "Disk Service needs to have 3 files");
                    
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
                    provider.Initialize();
                    service.AddProvider(provider);

                    Assert.True(false, "Todo");

                    service.RemoveProvider(provider);
                    Assert.AreEqual(0, service.GetFileEntries().Count, "After removal service must have no files");
                }
            }
        }
    }
}
