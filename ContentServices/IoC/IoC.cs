namespace CarbonCore.ContentServices.IoC
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class ContentServicesCompatModule : CarbonQuickModule
    {
        public ContentServicesCompatModule()
        {
            this.For<IFileService>().Use<FileService>();

            this.For<IJsonDatabaseService>().Use<JsonDatabaseService>();

            this.For<IFileServiceMemoryProvider>().Use<FileServiceMemoryProvider>();
            this.For<IFileServiceDiskProvider>().Use<FileServiceDiskProvider>();
            this.For<IFileServicePackProvider>().Use<FileServicePackProvider>();
        }
    }
}