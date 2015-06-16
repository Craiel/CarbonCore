namespace CarbonCore.ContentServices.IoC
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class ContentServicesModule : CarbonQuickModule
    {
        public ContentServicesModule()
        {
            this.For<ISqlLiteConnector>().Use<SqlLiteConnector>();

            this.For<IDatabaseService>().Use<DatabaseService>();
            this.For<IFileService>().Use<FileService>();

            this.For<IFileServiceMemoryProvider>().Use<FileServiceMemoryProvider>();
            this.For<IFileServiceDiskProvider>().Use<FileServiceDiskProvider>();
            this.For<IFileServicePackProvider>().Use<FileServicePackProvider>();
        }
    }
}