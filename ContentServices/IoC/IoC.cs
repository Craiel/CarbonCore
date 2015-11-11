namespace CarbonCore.ContentServices.IoC
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.IoC;
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(ContentServicesCompatModule))]
    [DependsOnModule(typeof(UtilsModule))]
    public class ContentServicesModule : CarbonQuickModule
    {
        public ContentServicesModule()
        {
            this.For<ISqlLiteConnector>().Use<SqlLiteConnector>();

            this.For<ISqlLiteDatabaseService>().Use<SqlLiteDatabaseService>();

            this.For<ISqlLiteFileServiceDiskProvider>().Use<SqlLiteFileServiceDiskProvider>();
            this.For<ISqlLiteFileServicePackProvider>().Use<SqlLiteFileServicePackProvider>();
        }
    }
}