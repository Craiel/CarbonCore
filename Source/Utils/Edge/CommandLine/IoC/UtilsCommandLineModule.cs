namespace CarbonCore.Utils.Edge.CommandLine.IoC
{
    using CarbonCore.GrammarParser.IoC;
    using CarbonCore.Utils.Edge.CommandLine.Contracts;
    using CarbonCore.Utils.Edge.CommandLine.Logic;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;
    
    [DependsOnModule(typeof(UtilsEdgeModule))]
    [DependsOnModule(typeof(GrammarParserModule))]
    public class UtilsCommandLineModule : CarbonQuickModule
    {
        public UtilsCommandLineModule()
        {
            this.For<ICommandLineArguments>().Use<CommandLineArguments>();
        }
    }
}