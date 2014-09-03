namespace CarbonCore.Tests.ContentServices
{
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

            IList<CarbonFile> files = this.GetType().Assembly.ExtractResources(this.dataDirectory, "Resources.FileEntries");
            Assert.AreEqual(3, files.Count, "Must extract all resources properly");
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

                service.AddProvider(new FileServiceMemoryProvider());
                service.AddProvider(new FileServiceDiskProvider(this.dataDirectory));
                service.AddProvider(new FileServicePackProvider());

                Assert.AreEqual(3, service.GetProviders().Count, "Must have all three providers registered");
            }
        }

        [Test]
        public void MemoryProviderTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                service.AddProvider(new FileServiceMemoryProvider());

                Assert.True(false, "Todo");
            }
        }

        [Test]
        public void DiskProviderTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                service.AddProvider(new FileServiceDiskProvider(this.dataDirectory));

                Assert.True(false, "Todo");
            }
        }

        [Test]
        public void PackProviderTests()
        {
            using (var service = this.container.Resolve<IFileService>())
            {
                service.AddProvider(new FileServicePackProvider());

                Assert.True(false, "Todo");
            }
        }
    }
}
