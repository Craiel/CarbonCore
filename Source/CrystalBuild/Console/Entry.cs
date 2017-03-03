namespace CarbonCore.Applications.CrystalBuild.CSharp
{
    using CarbonCore.Applications.CrystalBuild.Contracts;
    using CarbonCore.Applications.CrystalBuild.CSharp.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Edge.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<CrystalBuildCSharpModule>();
            container.Resolve<IMain>().Start();

            Profiler.TraceProfilerStatistics();
        }
    }
}