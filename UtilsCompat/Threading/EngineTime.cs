namespace CarbonCore.Utils.Compat.Threading
{
    using System.Diagnostics;

    public class EngineTime
    {
        private static readonly Stopwatch TimeSinceStart;

        private long lastUpdateTicks;

        private long ticksSinceLastFrame;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static EngineTime()
        {
            TimeSinceStart = new Stopwatch();
            TimeSinceStart.Start();
        }

        public EngineTime()
        {
            this.Reset();
        }

        public EngineTime(double time)
            : this()
        {
            this.Ticks = (long)(time * Stopwatch.Frequency);
        }

        public EngineTime(long frame, float speed, long ticks, long fixedTicks, long ticksLostToPause)
            : this()
        {
            this.Frame = frame;
            this.Speed = speed;
            this.Ticks = ticks;
            this.FixedTicks = fixedTicks;
            this.TicksLostToPause = ticksLostToPause;

            // Call a single update with the ticks to fill all the other values
            this.DoUpdate(this.lastUpdateTicks);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsPaused { get; private set; }

        public long Frame { get; private set; }

        public long Ticks { get; private set; }

        public long DeltaTicks { get; private set; }

        public long FixedTicks { get; private set; }

        public long FixedDeltaTicks { get; private set; }

        public long TicksLostToPause { get; private set; }

        public long FrameDeltaTicks { get; private set; }

        public double Time { get; private set; }

        public double DeltaTime { get; private set; }

        public double FixedTime { get; private set; }

        public double TimeLostToPause { get; private set; }

        public double FrameDeltaTime { get; private set; }

        public float Speed { get; private set; }

        public void Pause()
        {
            this.IsPaused = true;
        }

        public void Resume()
        {
            this.IsPaused = false;
        }

        public void ChangeSpeed(float speed)
        {
            this.Speed = speed;
        }
        
        public void Reset()
        {
            this.IsPaused = false;

            // Store the start point for the time
            this.lastUpdateTicks = TimeSinceStart.ElapsedTicks;

            this.Frame = 0;
            this.Ticks = 0;
            this.DeltaTicks = 0;
            this.FixedTicks = 0;
            this.FixedDeltaTicks = 0;
            this.TicksLostToPause = 0;
            this.FrameDeltaTicks = 0;

            this.Time = 0;
            this.DeltaTime = 0;
            this.FixedTime = 0;
            this.TimeLostToPause = 0;
            this.FrameDeltaTime = 0;

            this.Speed = 1.0f;

            this.ticksSinceLastFrame = 0;
        }

        public void Update()
        {
            this.DoUpdate(TimeSinceStart.ElapsedTicks);
        }

        public void UpdateFrame()
        {
            if (this.IsPaused)
            {
                // No frame updates when the timer is paused
                return;
            }

            this.Frame++;

            this.FrameDeltaTicks = this.ticksSinceLastFrame;
            this.ticksSinceLastFrame = 0;

            this.FrameDeltaTime = (double)this.FrameDeltaTicks / Stopwatch.Frequency;
        }

        public EngineTime Clone()
        {
            var clone = new EngineTime();
            clone.CopyFrom(this);
            return clone;
        }

        public void CopyFrom(EngineTime other)
        {
            this.Frame = other.Frame;

            this.Speed = other.Speed;

            this.Ticks = other.Ticks;
            this.Time = other.Time;

            this.FixedTicks = other.FixedTicks;
            this.FixedTime = other.FixedTime;
            this.FixedDeltaTicks = other.FixedDeltaTicks;

            this.DeltaTime = other.DeltaTime;
            this.DeltaTicks = other.DeltaTicks;

            this.FrameDeltaTicks = other.FrameDeltaTicks;
            this.FrameDeltaTime = other.FrameDeltaTime;

            this.TicksLostToPause = other.TicksLostToPause;
            this.TimeLostToPause = other.TimeLostToPause;
        }

        private void DoUpdate(long fixedElapsedTicks)
        {
            // Update the fixed time values first
            this.FixedDeltaTicks = fixedElapsedTicks - this.lastUpdateTicks;
            this.FixedTicks += this.FixedDeltaTicks;
            this.lastUpdateTicks = fixedElapsedTicks;

            // Now get the adjusted delta ticks based on the speed
            long deltaTicks = (long)(this.FixedDeltaTicks * this.Speed);

            // Check if the time is paused
            if (this.IsPaused)
            {
                this.DeltaTicks = 0;
                this.TicksLostToPause += deltaTicks;
            }
            else
            {
                this.Ticks += deltaTicks;
                this.DeltaTicks = deltaTicks;
                this.ticksSinceLastFrame += deltaTicks;
            }

            // Recalculate the time values
            this.Time = (double)this.Ticks / Stopwatch.Frequency;
            this.FixedTime = (double)this.FixedTicks / Stopwatch.Frequency;
            this.DeltaTime = (double)this.DeltaTicks / Stopwatch.Frequency;
            this.TimeLostToPause = (double)this.TicksLostToPause / Stopwatch.Frequency;
        }
    }
}