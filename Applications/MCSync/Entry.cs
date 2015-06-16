namespace MCSync
{
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.IoC;
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
            container.Resolve<IMain>().Sync();

            Profiler.TraceProfilerStatistics();
        }
    }
}
