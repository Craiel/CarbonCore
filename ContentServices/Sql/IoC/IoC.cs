namespace CarbonCore.ContentServices.Sql.IoC
{
    using CarbonCore.ContentServices.IoC;
    using CarbonCore.ContentServices.Sql.Contracts;
    using CarbonCore.ContentServices.Sql.Logic;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(ContentServicesModule))]
    [DependsOnModule(typeof(UtilsModule))]
    public class ContentServicesSqlModule : CarbonQuickModule
    {
        public ContentServicesSqlModule()
        {
            this.For<IFileService>().Use<FileService>();

            this.For<IJsonDatabaseService>().Use<JsonDatabaseService>();

            this.For<IFileServiceMemoryProvider>().Use<FileServiceMemoryProvider>();
            this.For<IFileServiceDiskProvider>().Use<FileServiceDiskProvider>();
            this.For<IFileServicePackProvider>().Use<FileServicePackProvider>();
        }
    }
}