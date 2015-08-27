namespace CarbonCore.Tests.Compat.Utils
{
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
            Assert.AreEqual(time.Ticks, time.FixedTicks);
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
        }
    }
}
