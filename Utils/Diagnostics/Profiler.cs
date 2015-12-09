namespace CarbonCore.Utils.Diagnostics
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;

    using Timer = CarbonCore.Utils.Timer;

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
                string name = region.IncludeThreadId
                                  ? string.Format("({0}) {1}", Thread.CurrentThread.ManagedThreadId, region.Name)
                                  : region.Name;

                if (!RegionStatistics.ContainsKey(name))
                {
                    statistics = new ProfileStatistics(name);
                    RegionStatistics.Add(name, statistics);
                }
                else
                {
                    statistics = (ProfileStatistics)RegionStatistics[name];
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
                    Timer.TimeToTimeSpan((long)statistic.Time),
                    Timer.TimeToTimeSpan((long)statistic.AverageTime).TotalMilliseconds);
            }
        }
    }
}
