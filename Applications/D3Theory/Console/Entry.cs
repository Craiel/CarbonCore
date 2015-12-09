namespace CarbonCore.Applications.D3Theory.Console
{
    using CarbonCore.Applications.D3Theory.Console.Contracts;
    using CarbonCore.Applications.D3Theory.Console.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Edge.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<D3TheoryConsoleModule>();
            container.Resolve<IMain>().Start();

            Profiler.TraceProfilerStatistics();
        }
    }
}
