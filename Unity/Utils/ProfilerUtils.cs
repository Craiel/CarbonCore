namespace CarbonCore.Utils.Unity
{
    using System.Diagnostics;
    using System.Threading;

    public static class ProfilerUtils
    {
        private static int mainThreadId;

        public static void Initialize()
        {
            mainThreadId = Thread.CurrentThread.ManagedThreadId;
        }

        [Conditional("ENABLE_PROFILER")]
        public static void BeginSampleThreadsafe(string title)
        {
#if !NUNIT
            if (Thread.CurrentThread.ManagedThreadId != mainThreadId)
            {
                return;
            }

            UnityEngine.Profiler.BeginSample(title);
#endif
        }

        [Conditional("ENABLE_PROFILER")]
        public static void EndSampleThreadSafe()
        {
#if !NUNIT
            if (Thread.CurrentThread.ManagedThreadId != mainThreadId)
            {
                return;
            }

            UnityEngine.Profiler.EndSample();
#endif
        }
    }
}
