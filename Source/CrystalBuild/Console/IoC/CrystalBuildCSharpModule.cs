namespace CarbonCore.CrystalBuild.Console.IoC
{
    using Applications.CrystalBuild.Contracts;

    using Contracts;
    
    using ToolFramework.Console.IoC;

    using Utils.Edge.CommandLine.IoC;
    using Utils.Edge.IoC;
    using Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    [DependsOnModule(typeof(ToolFrameworkConsoleModule))]
    public class CrystalBuildCSharpModule : CarbonQuickModule
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CrystalBuildCSharpModule()
        {
            this.For<IMain>().Use<Main>();
            this.For<IConfig>().Use<Config>();
        }
    }
}