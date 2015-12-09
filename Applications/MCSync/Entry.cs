namespace CarbonCore.Applications.MCSync
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IoC;
    
    using MCSync.Contracts;
    using MCSync.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<MCSyncModule>();
            container.Resolve<IMain>().Start();

            Profiler.TraceProfilerStatistics();
        }
    }
}
