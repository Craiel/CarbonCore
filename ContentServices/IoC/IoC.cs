namespace CarbonCore.ContentServices.IoC
{
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class ContentServicesModule : CarbonModule
    {
        public ContentServicesModule()
        {
        }
    }
}