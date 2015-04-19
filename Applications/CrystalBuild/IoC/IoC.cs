namespace CarbonCore.Applications.CrystalBuild.IoC
{
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsCommandLine.IoC;

    using CrystalBuild.Contracts;
    using CrystalBuild.Contracts.Processors;
    using CrystalBuild.Logic;
    using CrystalBuild.Logic.Processors;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    public class CrystalBuildModule : CarbonModule
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
            this.For<IExcelProcessor>().Use<ExcelProcessor>();
            this.For<ITemplateProcessor>().Use<TemplateProcessor>();
            this.For<IJavaScriptProcessor>().Use<JavaScriptProcessor>();
            this.For<ICssProcessor>().Use<CssProcessor>();
            this.For<IImageProcessor>().Use<ImageProcessor>();
        }
    }
}