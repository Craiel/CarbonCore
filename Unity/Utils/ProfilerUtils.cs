namespace CarbonCore.Unity.Utils
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    public static class ProfilerUtils
    {
        private static int mainThreadId;

        private static Action<string> beginCallback;
        private static Action endCallback;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool IsEnabled { get; set; }

        [Conditional("ENABLE_PROFILER")]
        public static void Initialize(Action<string> begin, Action end)
        {
            beginCallback += begin;
            endCallback += end;

            mainThreadId = Thread.CurrentThread.ManagedThreadId;
            IsEnabled = true;
        }

        [Conditional("ENABLE_PROFILER")]
        public static void BeginSampleThreadsafe(string title)
        {
            if (!IsEnabled || Thread.CurrentThread.ManagedThreadId != mainThreadId)
            {
                return;
            }

            if (beginCallback != null)
            {
                beginCallback(title);
            }
        }

        [Conditional("ENABLE_PROFILER")]
        public static void EndSampleThreadSafe()
        {
            if (!IsEnabled || Thread.CurrentThread.ManagedThreadId != mainThreadId)
            {
                return;
            }

            if (endCallback != null)
            {
                endCallback();
            }
        }
    }
}
