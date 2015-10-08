namespace D3Theory.Console
{
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.IoC;

    using D3Theory.Console.Contracts;
    using D3Theory.Console.IoC;
    
    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<D3TheoryConsoleModule>();
            container.Resolve<IMain>().Simulate();

            Profiler.TraceProfilerStatistics();
        }
    }
}
