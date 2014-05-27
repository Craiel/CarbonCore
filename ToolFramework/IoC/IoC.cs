namespace CarbonCore.ToolFramework.IoC
{
    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Logic;
    using CarbonCore.ToolFramework.ViewModel;
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsWPF.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(UtilsWPFModule))]
    public class ToolFrameworkModule : CarbonModule
    {
        public ToolFrameworkModule()
        {
            this.For<IToolActionResult>().Use<ToolActionResult>();

            this.For<IToolActionViewModel>().Use<ToolActionViewModel>();
            this.For<IToolActionDialogViewModel>().Use<ToolActionDialogViewModel>();
        }
    }
}