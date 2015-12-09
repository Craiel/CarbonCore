namespace CarbonCore.Applications.MCSync.IoC
{
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.Edge.CommandLine.IoC;

    using MCSync.Contracts;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    [DependsOnModule(typeof(ToolFrameworkModule))]
    public class MCSyncModule : CarbonQuickModule
    {
        public MCSyncModule()
        {
            this.For<IMain>().Use<Main>();
        }
    }
}