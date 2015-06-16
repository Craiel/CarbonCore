﻿namespace CarbonCore.UtilsCommandLine.IoC
{
    using CarbonCore.GrammarParser.IoC;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsCommandLine.Contracts;
    using CarbonCore.UtilsCommandLine.Logic;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(GrammarParserModule))]
    public class UtilsCommandLineModule : CarbonModuleAutofac
    {
        public UtilsCommandLineModule()
        {
            this.For<ICommandLineArguments>().Use<CommandLineArguments>();
        }
    }
}