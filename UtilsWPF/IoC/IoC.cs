namespace CarbonCore.UtilsWPF.IoC
{
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class UtilsWPFModule : CarbonModule
    {
        public UtilsWPFModule()
        {
        }
    }
}