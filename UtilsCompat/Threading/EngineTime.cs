namespace CarbonCore.Utils.Compat.Threading
{
    using System.Diagnostics;

    public class EngineTime
    {
        private static readonly Stopwatch TimeSinceStart;

        private long initialTicks;

        private long ticksSineLastFrame;
        
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

            // Save the initial ticks so we don't spike against the static watch
            this.initialTicks = TimeSinceStart.ElapsedTicks;
        }

        public EngineTime(long frame, float speed, long fixedTicks, long ticksLostToPause)
            : this()
        {
            this.Frame = frame;
            this.Speed = speed;
            this.FixedTicks = fixedTicks;
            this.TicksLostToPause = ticksLostToPause;

            // Call a single update with the ticks to fill all the other values
            this.DoUpdate(fixedTicks);
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

            this.initialTicks = 0;

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

            this.ticksSineLastFrame = 0;
        }

        public void Update()
        {
            // Update the fixed time values first
            long fixedElapsedTicks = this.initialTicks + TimeSinceStart.ElapsedTicks;
            this.DoUpdate(fixedElapsedTicks);
        }

        public void UpdateFrame()
        {
            if (this.IsPaused)
            {
                // No frame updates when the timer is paused
                return;
            }

            this.Frame++;

            this.FrameDeltaTicks = this.ticksSineLastFrame;
            this.ticksSineLastFrame = 0;

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
            this.FixedDeltaTicks = fixedElapsedTicks - this.FixedTicks;
            this.FixedTicks = fixedElapsedTicks;

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
                this.ticksSineLastFrame += deltaTicks;
            }

            // Recalculate the time values
            this.Time = (double)this.Ticks / Stopwatch.Frequency;
            this.FixedTime = (double)this.FixedTicks / Stopwatch.Frequency;
            this.DeltaTime = (double)this.DeltaTicks / Stopwatch.Frequency;
            this.TimeLostToPause = (double)this.TicksLostToPause / Stopwatch.Frequency;
        }
    }
}