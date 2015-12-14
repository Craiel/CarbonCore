namespace Assets.Scripts.Tests.General
{
    using System.IO;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.IO;

    public static class GeneralTest
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Run()
        {
            MetricTime measure = Diagnostic.BeginTimeMeasure();

            TestCarbonDirectory();
            TestCarbonFile();

            Diagnostic.TakeTimeMeasure(measure);
            Diagnostic.Info("General test finished in {0}ms", Diagnostic.GetTimeInMS(measure.Total));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void TestCarbonDirectory()
        {
            var root = new CarbonDirectory(UnityEngine.Application.dataPath);
            
            CarbonDirectoryResult[] results = root.GetDirectories(options: SearchOption.AllDirectories);
            foreach (CarbonDirectoryResult result in results)
            {
            }
        }

        private static void TestCarbonFile()
        {
            var root = new CarbonDirectory(UnityEngine.Application.dataPath);

            CarbonFileResult[] results = root.GetFiles(options: SearchOption.AllDirectories);
            long totalSize = 0;
            foreach (CarbonFileResult result in results)
            {
                totalSize += result.Absolute.Size;
            }
        }
    }
}
