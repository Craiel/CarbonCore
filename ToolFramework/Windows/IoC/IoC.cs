namespace CarbonCore.ToolFramework.Windows.IoC
{
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.ToolFramework.Windows.Contracts.ViewModels;
    using CarbonCore.ToolFramework.Windows.ViewModel;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.Edge.WPF.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(UtilsWPFModule))]
    [DependsOnModule(typeof(ToolFrameworkModule))]
    public class ToolFrameworkWindowsModule : CarbonQuickModule
    {
        public ToolFrameworkWindowsModule()
        {
            this.For<IToolActionViewModel>().Use<ToolActionViewModel>();
            this.For<IToolActionDialogViewModel>().Use<ToolActionDialogViewModel>();

            this.For<ILogViewModel>().Use<LogViewModel>();
            this.For<ILogEntryViewModel>().Use<LogEntryViewModel>();
        }
    }
}