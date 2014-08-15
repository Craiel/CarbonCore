namespace CarbonCore.UtilsCommandLine.IoC
{
    using CarbonCore.GrammarParser.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsCommandLine.Contracts;
    using CarbonCore.UtilsCommandLine.Logic;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(GrammarParserModule))]
    public class UtilsCommandLineModule : CarbonModule
    {
        public UtilsCommandLineModule()
        {
            this.For<ICommandLineArguments>().Use<CommandLineArguments>();
        }
    }
}