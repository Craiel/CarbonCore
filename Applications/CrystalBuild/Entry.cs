namespace CarbonCore.Applications.CrystalBuild
{
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.IoC;

    using CrystalBuild.Contracts;
    using CrystalBuild.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerBuilder.Build<CrystalBuildModule>();
            container.Resolve<IMain>().Build();

            Profiler.TraceProfilerStatistics();
        }
    }
}