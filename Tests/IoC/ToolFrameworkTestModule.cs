namespace CarbonCore.Tests.IoC
{
    using CarbonCore.Tests.Contracts;
    using CarbonCore.Tests.ToolFramework;
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsWPF.IoC;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(UtilsWPFModule))]
    [DependsOnModule(typeof(ToolFrameworkModule))]
    public class ToolFrameworkTestModule : CarbonModuleAutofac
    {
        public ToolFrameworkTestModule()
        {
            this.For<IFrameworkTestMain>().Use<FrameworkTestMain>().Singleton();

            this.For<IFrameworkTestMainViewModel>().Use<FrameworkTestMainViewModel>();
        }
    }
}
