namespace CarbonCore.GrammarParser.IoC
{
    using CarbonCore.GrammarParser.Contracts.Grammars;
    using CarbonCore.GrammarParser.Grammars;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    [DependsOnModule(typeof(UtilsModule))]
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