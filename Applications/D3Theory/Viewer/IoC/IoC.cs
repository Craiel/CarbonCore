namespace D3Theory.Viewer.IoC
{
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.Utils.IoC;

    using D3Theory.Viewer.Contracts;
    using D3Theory.Viewer.ViewModel;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(ToolFrameworkModule))]
    public class D3TheoryViewerModule : CarbonQuickModule
    {
        public D3TheoryViewerModule()
        {
            this.For<ID3ViewerMain>().Use<D3ViewerMain>().Singleton();

            this.For<ID3ViewerMainViewModel>().Use<D3ViewerMainViewModel>();
        }
    }
}