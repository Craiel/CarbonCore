namespace CarbonCore.Tests.Compat.Utils
{
    using System.Diagnostics;
    using System.Threading;

    using CarbonCore.Utils.Compat.Threading;

    using NUnit.Framework;

    [TestFixture]
    public class EngineTimeTests
    {
        private const int TestSampleCount = 10;

        [Test]
        public void TestGeneralTimeUpdate()
        {
            var time = new EngineTime();
            Assert.AreEqual(time.Ticks, 0);
            Assert.AreEqual(time.FixedTicks, 0);

            // Check that update properly updates the ticks
            time.Update();
            Assert.Greater(time.Ticks, 0);
            Assert.Greater(time.DeltaTicks, 0);
            Assert.LessOrEqual(time.FixedTicks - time.Ticks, 5);
        }

        [Test]
        public void TestPause()
        {
            var time = new EngineTime();
            Assert.IsFalse(time.IsPaused);

            // Check that pausing the time will only update the fixed or lost values
            time.Pause();
            Assert.AreEqual(time.Ticks, 0);
            Assert.AreEqual(time.FixedTicks, 0);
            time.Update();
            Assert.AreEqual(time.Ticks, 0);
            Assert.Greater(time.FixedTicks, 0);
            Assert.Greater(time.TicksLostToPause, 0);
        }

        [Test]
        public void TestTimeSpeedFactor()
        {
            var time = new EngineTime();

            for (int i = 0; i < TestSampleCount; i++)
            {
                Thread.Sleep(1);
                time.Update();
            }

            long normalSpeedTicks = time.Ticks;

            time.Reset();
            time.ChangeSpeed(2.0f);

            for (int i = 0; i < TestSampleCount; i++)
            {
                Thread.Sleep(1);
                time.Update();
            }

            long doubleSpeedTicks = time.Ticks;

            time.Reset();
            time.ChangeSpeed(0.5f);

            for (int i = 0; i < TestSampleCount; i++)
            {
                Thread.Sleep(1);
                time.Update();
            }

            long halfSpeedTicks = time.Ticks;

            Assert.Greater(doubleSpeedTicks, normalSpeedTicks);
            Assert.Less(halfSpeedTicks, normalSpeedTicks);
        }

        [Test]
        public void TestFrameUpdate()
        {
            var time = new EngineTime();
            Assert.AreEqual(time.Frame, 0);

            for (int i = 0; i < TestSampleCount; i++)
            {
                Thread.Sleep(1);
                time.Update();
            }

            Assert.AreEqual(time.Frame, 0);
            Assert.AreEqual(time.FrameDeltaTicks, 0);

            time.UpdateFrame();

            Assert.AreEqual(time.Frame, 1);
            Assert.Greater(time.FrameDeltaTicks, 0);
            Assert.Greater(time.FrameDeltaTime, 0);

            time.Pause();
            time.UpdateFrame();

            Assert.AreEqual(time.Frame, 1);
        }

        [Test]
        public void TestTimeSaveRestore()
        {
            var time = new EngineTime(500, 2.0f, 400000000, 200000000, 500);
            Assert.AreEqual(500, time.Frame);
            Assert.AreEqual(200000000, time.FixedTicks);

            for (int i = 0; i < TestSampleCount; i++)
            {
                Thread.Sleep(1);
                time.Update();
                Assert.LessOrEqual(time.DeltaTicks, Stopwatch.Frequency / 10f);
            }

            Assert.Greater(time.Time, 0);
            Assert.LessOrEqual(time.FrameDeltaTicks, Stopwatch.Frequency);

            time.UpdateFrame();
            Assert.AreEqual(501, time.Frame);

            var time2 = new EngineTime(500, 2.0f, 400000000, 200000000, 500);
            Assert.Less(time2.Time, time.Time);
        }
    }
}
