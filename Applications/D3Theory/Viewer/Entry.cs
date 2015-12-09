namespace D3Theory.Viewer
{
    using System;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IoC;

    using D3Theory.Viewer.Contracts;
    using D3Theory.Viewer.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [STAThread]
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<D3TheoryViewerModule>();
            container.Resolve<ID3ViewerMain>().Start();

            Profiler.TraceProfilerStatistics();
        }
    }
}
