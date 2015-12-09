namespace CarbonCore.ContentServices.Edge.IoC
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Edge.Contracts;
    using CarbonCore.ContentServices.Edge.Logic;
    using CarbonCore.ContentServices.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;
    
    [DependsOnModule(typeof(ContentServicesCompatModule))]
    [DependsOnModule(typeof(UtilsEdgeModule))]
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