namespace CarbonCore.Applications.MCSync.IoC
{
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.Utils.Edge.CommandLine.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;

    using MCSync.Contracts;

    [DependsOnModule(typeof(UtilsEdgeModule))]
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