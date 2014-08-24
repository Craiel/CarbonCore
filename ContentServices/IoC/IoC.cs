namespace CarbonCore.ContentServices.IoC
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class ContentServicesModule : CarbonModule
    {
        public ContentServicesModule()
        {
            this.For<ISqlLiteConnector>().Use<SqlLiteConnector>();

            this.For<IDatabaseService>().Use<DatabaseService>();
        }
    }
}