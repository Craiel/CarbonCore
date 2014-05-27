namespace CarbonCore.Utils.Diagnostics
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public static class Profiler
    {
        private static readonly Hashtable RegionStatistics;

        static Profiler()
        {
            RegionStatistics = new Hashtable();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void RegionStart(ProfileRegion region)
        {
        }

        public static void RegionFinish(ProfileRegion region)
        {
            ProfileStatistics statistics;
            lock (RegionStatistics)
            {
                if (!RegionStatistics.ContainsKey(region.Name))
                {
                    statistics = new ProfileStatistics(region.Name);
                    RegionStatistics.Add(region.Name, statistics);
                }
                else
                {
                    statistics = (ProfileStatistics)RegionStatistics[region.Name];
                }
            }

            statistics.AddCount(1);
            statistics.AddTime(region.ElapsedTicks);
        }
        
        public static void TraceProfilerStatistics()
        {
            var statistics = new ProfileStatistics[RegionStatistics.Values.Count];
            RegionStatistics.Values.CopyTo(statistics, 0);
            IList<ProfileStatistics> bla = new List<ProfileStatistics>(statistics).OrderBy(x => x.Name).ToList();
            foreach (ProfileStatistics statistic in bla)
            {
                System.Diagnostics.Trace.TraceInformation(
                    "Region Statistic - {0}: Count {1}, Total Time {2}, Average {3:f2}ms",
                    statistic.Name, 
                    statistic.Count, 
                    Timer.CounterToTimeSpan((long)statistic.Time).ToString(@"hh\:mm\:ss\:fff"),
                    Timer.CounterToTimeSpan((long)statistic.AverageTime).TotalMilliseconds);
            }
        }
    }
}
