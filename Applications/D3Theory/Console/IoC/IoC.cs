namespace CarbonCore.Applications.D3Theory.Console.IoC
{
    using CarbonCore.Applications.D3Theory.Console.Contracts;
    using CarbonCore.Modules.D3Theory.IoC;
    using CarbonCore.Utils.Edge.CommandLine.IoC;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;
    
    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    [DependsOnModule(typeof(D3TheoryModule))]
    public class D3TheoryConsoleModule : CarbonQuickModule
    {
        public D3TheoryConsoleModule()
        {
            this.For<IMain>().Use<Main>();
        }
    }
}