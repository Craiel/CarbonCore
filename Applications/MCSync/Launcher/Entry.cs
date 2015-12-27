namespace CarbonCore.Applications.MCSync.Launcher
{
    using CarbonCore.Applications.MCSync.Launcher.Contracts;
    using CarbonCore.Applications.MCSync.Launcher.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Edge.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<MCSyncLauncherModule>();
            container.Resolve<IMain>().Start();

            Profiler.TraceProfilerStatistics();
        }
    }
}
