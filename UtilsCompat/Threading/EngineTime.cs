namespace CarbonCore.Utils.Compat.Threading
{
    using System.Diagnostics;

    public class EngineTime
    {
        private static readonly Stopwatch TimeSinceStart;

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
            long fixedElapsedTicks = TimeSinceStart.ElapsedTicks;
            this.FixedDeltaTicks = fixedElapsedTicks - this.FixedTicks;
            this.FixedTicks = fixedElapsedTicks;
            this.FixedTime = (double)this.FixedTicks / Stopwatch.Frequency;

            // Now get the adjusted delta ticks based on the speed
            long deltaTicks = (long)(this.FixedDeltaTicks * this.Speed);

            // Check if the time is paused
            if (this.IsPaused)
            {
                this.DeltaTicks = 0;
                this.TicksLostToPause += deltaTicks;

                this.TimeLostToPause = (double)this.TicksLostToPause / Stopwatch.Frequency;
            }
            else
            {
                this.Ticks += deltaTicks;
                this.DeltaTicks = deltaTicks;
                this.ticksSineLastFrame += deltaTicks;

                this.Time = (double)this.Ticks / Stopwatch.Frequency;
                this.DeltaTime = (double)this.DeltaTicks / Stopwatch.Frequency;
                
            }
        }

        public void UpdateFrame()
        {
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
            this.Ticks = other.Ticks;
            this.Time = other.Time;

            this.FixedTicks = other.FixedTicks;
            this.FixedTime = other.FixedTime;

            this.DeltaTime = other.DeltaTime;
            this.DeltaTicks = other.DeltaTicks;

            this.FrameDeltaTicks = other.FrameDeltaTicks;
            this.FrameDeltaTime = other.FrameDeltaTime;
        }
    }
}