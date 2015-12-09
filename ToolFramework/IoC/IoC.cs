namespace CarbonCore.ToolFramework.IoC
{
    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Logic;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    public class ToolFrameworkModule : CarbonQuickModule
    {
        public ToolFrameworkModule()
        {
            this.For<IToolActionResult>().Use<ToolActionResult>();
        }
    }
}