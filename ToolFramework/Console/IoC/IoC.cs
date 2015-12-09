namespace CarbonCore.ToolFramework.Console.IoC
{
    using CarbonCore.ToolFramework.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(ToolFrameworkModule))]
    public class ToolFrameworkConsoleModule : CarbonQuickModule
    {
    }
}