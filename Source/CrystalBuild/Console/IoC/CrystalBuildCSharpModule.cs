namespace CarbonCore.CrystalBuild.Console.IoC
{
    using Applications.CrystalBuild.Contracts;
    using Applications.CrystalBuild.CSharp;
    using Applications.CrystalBuild.CSharp.Contracts;
    using Applications.CrystalBuild.CSharp.Logic;

    using Contracts;

    using CrystalBuild.IoC;

    using ToolFramework.Console.IoC;

    using Utils.Edge.CommandLine.IoC;
    using Utils.Edge.IoC;
    using Utils.IoC;

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