namespace CarbonCore.ToolFramework.IoC
{
    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.ToolFramework.Logic;
    using CarbonCore.ToolFramework.ViewModel;
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.Edge.WPF.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(UtilsWPFModule))]
    public class ToolFrameworkModule : CarbonQuickModule
    {
        public ToolFrameworkModule()
        {
            this.For<IToolActionResult>().Use<ToolActionResult>();

            this.For<IToolActionViewModel>().Use<ToolActionViewModel>();
            this.For<IToolActionDialogViewModel>().Use<ToolActionDialogViewModel>();

            this.For<ILogViewModel>().Use<LogViewModel>();
            this.For<ILogEntryViewModel>().Use<LogEntryViewModel>();
        }
    }
}