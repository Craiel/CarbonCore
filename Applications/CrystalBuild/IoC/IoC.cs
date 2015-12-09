namespace CarbonCore.Applications.CrystalBuild.IoC
{
    using CarbonCore.Applications.CrystalBuild.Logic.Processors.Excel;
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.Utils.Edge.CommandLine.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;

    using CrystalBuild.Contracts;
    using CrystalBuild.Contracts.Processors;
    using CrystalBuild.Logic;
    using CrystalBuild.Logic.Processors;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    [DependsOnModule(typeof(ToolFrameworkModule))]
    public class CrystalBuildModule : CarbonQuickModule
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CrystalBuildModule()
        {
            this.For<IMain>().Use<Main>();
            this.For<IConfig>().Use<Config>();

            this.For<IBuildUtils>().Use<BuildUtils>();

            this.For<IBuildLogic>().Use<BuildLogic>();
            this.For<IExcelProcessor>().Use<CrystalExcelProcessor>();
            this.For<ITemplateProcessor>().Use<TemplateProcessor>();
            this.For<IJavaScriptProcessor>().Use<JavaScriptProcessor>();
            this.For<ICssProcessor>().Use<CssProcessor>();
            this.For<IImageProcessor>().Use<ImageProcessor>();
        }
    }
}