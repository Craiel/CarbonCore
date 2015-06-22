namespace CarbonCore.Utils.IoC
{
    using CarbonCore.Utils.Compat.IoC;

    [DependsOnModule(typeof(UtilsCompatModule))]
    public class UtilsModule : CarbonQuickModule
    {
        public UtilsModule()
        {
        }
    }
}