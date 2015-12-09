namespace CarbonCore.Applications.D3Theory.Viewer.IoC
{
    using CarbonCore.Applications.D3Theory.Viewer;
    using CarbonCore.Applications.D3Theory.Viewer.Contracts;
    using CarbonCore.Applications.D3Theory.Viewer.ViewModel;
    using CarbonCore.ToolFramework.Windows.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(ToolFrameworkWindowsModule))]
    public class D3TheoryViewerModule : CarbonQuickModule
    {
        public D3TheoryViewerModule()
        {
            this.For<ID3ViewerMain>().Use<D3ViewerMain>().Singleton();

            this.For<ID3ViewerMainViewModel>().Use<D3ViewerMainViewModel>();
        }
    }
}