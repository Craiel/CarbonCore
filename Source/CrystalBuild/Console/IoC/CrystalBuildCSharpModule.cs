namespace CarbonCore.Applications.CrystalBuild.CSharp.IoC
{
    using CarbonCore.Applications.CrystalBuild.Contracts;
    using CarbonCore.Applications.CrystalBuild.CSharp;
    using CarbonCore.Applications.CrystalBuild.CSharp.Contracts;
    using CarbonCore.Applications.CrystalBuild.CSharp.Logic;
    using CarbonCore.CrystalBuild.IoC;
    using CarbonCore.ToolFramework.Console.IoC;
    using CarbonCore.Utils.Edge.CommandLine.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    [DependsOnModule(typeof(ToolFrameworkConsoleModule))]
    [DependsOnModule(typeof(CrystalBuildModule))]
    public class CrystalBuildCSharpModule : CarbonQuickModule
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CrystalBuildCSharpModule()
        {
            this.For<IMain>().Use<Main>();
            this.For<IConfig>().Use<Config>();
            
            this.For<IBuildLogic>().Use<BuildLogic>();
        }
    }
}