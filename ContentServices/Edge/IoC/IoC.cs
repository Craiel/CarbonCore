namespace CarbonCore.ContentServices.Edge.IoC
{
    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.IoC;
    using CarbonCore.ContentServices.Edge.Contracts;
    using CarbonCore.ContentServices.Edge.Logic;
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