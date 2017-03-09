namespace CarbonCore.Tests.Utils
{
    using System.Threading;
    
    using CarbonCore.Utils.Threading;

    using NLog;

    using NUnit.Framework;

    [TestFixture]
    public class EngineThreadTests
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private int threadUpdateCount;
        private int threadUpdateResult;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [Test]
        public void TestThreadTiming()
        {
            this.TimeThread(60, 5);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void TimeThread(int targetFrameRate, int seconds)
        {
            int sleepTime = seconds * 1000;
            this.threadUpdateCount = 0;
            var thread = new EngineThread(
                this.TestThreadMain,
                "Test Thread",
                new EngineThreadSettings { ThrottleFrameRate = true, TargetFrameRate = targetFrameRate });

            thread.Start();

            // Reset the metrics synchronized with the thread
            thread.Synchronize(this.ResetMetrics);
            Thread.Sleep(sleepTime);

            // Ensure we keep the metrics only until this point
            thread.Synchronize(this.FinalizeMetrics);

            // Shutdown the thread properly
            thread.Shutdown();

            int optimalFramerate = targetFrameRate * seconds;
            Logger.Info("Running {0}s @ {1}fps, Result: {2}fps, optimal: {3}fps", seconds, targetFrameRate, this.threadUpdateResult, optimalFramerate);
        }

        private void ResetMetrics()
        {
            this.threadUpdateCount = 0;
            this.threadUpdateResult = 0;
        }

        private void FinalizeMetrics()
        {
            this.threadUpdateResult = this.threadUpdateCount;
        }

        private bool TestThreadMain(EngineTime time)
        {
            this.threadUpdateCount++;
            // Logger.Info("{0} {1} / {2}: {3}", time.Time, time.Ticks, TimeSpan.TicksPerSecond * time.Time, this.threadUpdateCount);
            return true;
        }
    }
}
