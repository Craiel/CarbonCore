namespace CarbonCore.Tests.Edge.IoC
{
    using CarbonCore.Tests.Edge.Contracts;
    using CarbonCore.Tests.Edge.ToolFramework;
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.Edge.WPF.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(UtilsWPFModule))]
    [DependsOnModule(typeof(ToolFrameworkModule))]
    public class ToolFrameworkTestModule : CarbonQuickModule
    {
        public ToolFrameworkTestModule()
        {
            this.For<IFrameworkTestMain>().Use<FrameworkTestMain>().Singleton();

            this.For<IFrameworkTestMainViewModel>().Use<FrameworkTestMainViewModel>();
        }
    }
}
