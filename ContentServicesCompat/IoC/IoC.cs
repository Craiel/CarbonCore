namespace CarbonCore.ContentServices.Compat.IoC
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.Logic;
    using CarbonCore.Utils.Compat.IoC;

    [DependsOnModule(typeof(UtilsCompatModule))]
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