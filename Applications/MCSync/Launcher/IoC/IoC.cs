namespace CarbonCore.Applications.MCSync.Launcher.IoC
{
    using CarbonCore.Applications.MCSync.Launcher.Contracts;
    using CarbonCore.ToolFramework.Console.IoC;
    using CarbonCore.Utils.Edge.CommandLine.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    [DependsOnModule(typeof(ToolFrameworkConsoleModule))]
    public class MCSyncLauncherModule : CarbonQuickModule
    {
        public MCSyncLauncherModule()
        {
            this.For<IMain>().Use<Main>();
            this.For<IConfig>().Use<Config>();
        }
    }
}