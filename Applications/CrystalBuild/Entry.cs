namespace CarbonCore.Applications.CrystalBuild
{
    using Autofac;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.IoC;
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
            var container = new CarbonContainerAutofacBuilder().Build<CrystalBuildModule>() as IContainer;
            container.Resolve<IMain>().Build();

            Profiler.TraceProfilerStatistics();
        }
    }
}