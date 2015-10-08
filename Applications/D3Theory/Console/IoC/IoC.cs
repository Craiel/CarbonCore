namespace D3Theory.Console.IoC
{
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsCommandLine.IoC;

    using D3Theory.Console;
    using D3Theory.Console.Contracts;
    using D3Theory.IoC;

    [DependsOnModule(typeof(UtilsModule))]
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