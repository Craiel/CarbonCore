namespace CarbonCore.Applications.CrystalBuild
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Edge.IoC;

    using CrystalBuild.Contracts;
    using CrystalBuild.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<CrystalBuildModule>();
            container.Resolve<IMain>().Build();

            Profiler.TraceProfilerStatistics();
        }
    }
}