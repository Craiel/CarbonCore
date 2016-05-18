namespace CarbonCore.GrammarParser.IoC
{
    using CarbonCore.GrammarParser.Contracts.Grammars;
    using CarbonCore.GrammarParser.Grammars;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;
    
    [DependsOnModule(typeof(UtilsEdgeModule))]
    public class GrammarParserModule : CarbonQuickModule
    {
        public GrammarParserModule()
        {
            // Grammars don't need to be instantiated, register as singletons
            this.For<IJavaGrammar>().Use<JavaGrammar>().Singleton();
            this.For<ICommandLineGrammar>().Use<CommandLineGrammar>().Singleton();
            this.For<ISqlGrammar>().Use<SqlGrammar>().Singleton();
        }
    }
}