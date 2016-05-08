namespace CarbonCore.CrystalBuild.IoC
{
    using CarbonCore.CrystalBuild.Contracts;
    using CarbonCore.CrystalBuild.Logic;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    public class CrystalBuildModule : CarbonQuickModule
    {
        public CrystalBuildModule()
        {
            this.For<ICrystalBuildConfigurationRunTime>().Use<CrystalBuildConfigurationRunTime>();
        }
    }
}