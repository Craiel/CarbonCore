namespace CarbonCore.Utils.Compat.Threading
{
    using System.Diagnostics;

    public class EngineTime
    {
        private static readonly Stopwatch TimeSinceStart;

        private long lastFrameTicks;
        
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
        public long Frame { get; private set; }

        public long Ticks { get; private set; }

        public long TickDeltaTicks { get; private set; }

        public long FrameDeltaTicks { get; private set; }

        public double Time { get; private set; }

        public double TickDeltaTime { get; private set; }

        public double FrameDeltaTime { get; private set; }
        
        public void Reset()
        {
            this.Frame = 0;

            this.Time = 0f;
            this.Ticks = TimeSinceStart.ElapsedTicks;
            this.lastFrameTicks = this.Ticks;

            this.TickDeltaTicks = 0;
            this.TickDeltaTime = 0f;

            this.FrameDeltaTicks = 0;
            this.FrameDeltaTime = 0f;
        }

        public void Update()
        {
            long elapsedTicks = TimeSinceStart.ElapsedTicks;
            this.TickDeltaTicks = elapsedTicks - this.Ticks;
            this.Ticks = elapsedTicks;

            double frequency = Stopwatch.Frequency;
            this.TickDeltaTime = this.TickDeltaTicks / frequency;
            this.Time = this.Ticks / frequency;
        }

        public void UpdateFrame()
        {
            this.Frame++;
            long elapsedTicks = TimeSinceStart.ElapsedTicks;
            this.FrameDeltaTicks = elapsedTicks - this.lastFrameTicks;
            this.lastFrameTicks = elapsedTicks;

            double frequency = Stopwatch.Frequency;
            this.FrameDeltaTime = this.FrameDeltaTicks / frequency;
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
            this.TickDeltaTime = other.TickDeltaTime;
            this.TickDeltaTicks = other.TickDeltaTicks;
            this.FrameDeltaTicks = other.FrameDeltaTicks;
        }
    }
}