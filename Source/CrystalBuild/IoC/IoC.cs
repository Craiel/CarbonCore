namespace CarbonCore.CrystalBuild.IoC
{
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class CrystalBuildModule : CarbonQuickModule
    {
        public CrystalBuildModule()
        {
        }
    }
}