namespace CarbonCore.Utils
{
    using System;
    using System.Diagnostics;

    using CarbonCore.Utils.Contracts;

    public class Timer : ITimer
    {
        public static readonly Timer CoreTimer = new Timer { AutoUpdate = true };

        private static readonly long Frequency = Stopwatch.Frequency;

        private long lastTime;

        private bool isLastTimeValid;

        private TimeSpan elapsedTime;
        private TimeSpan actualElapsedTime;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Timer()
        {
            this.TimeModifier = 1.0f;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float TimeModifier { get; set; }

        public TimeSpan ElapsedTime
        {
            get
            {
                if (this.AutoUpdate)
                {
                    this.Update();
                }

                return this.elapsedTime;
            }
        }

        public TimeSpan ActualElapsedTime
        {
            get
            {
                if (this.AutoUpdate)
                {
                    this.Update();
                }

                return this.actualElapsedTime;
            }
        }

        public TimeSpan TimeLostToPause
        {
            get
            {
                return this.ActualElapsedTime - this.ElapsedTime;
            }
        }

        public bool AutoUpdate { get; set; }
        public bool IsPaused { get; set; }

        public static TimeSpan CounterToTimeSpan(float delta)
        {
            return TimeSpan.FromTicks((long)(delta * 10000000.0f) / Frequency);
        }

        public void Reset()
        {
            this.elapsedTime = TimeSpan.Zero;
            this.actualElapsedTime = TimeSpan.Zero;
        }

        public void Pause()
        {
            this.IsPaused = true;
        }

        public void Resume()
        {
            this.IsPaused = false;
        }

        public void Update()
        {
            long time = Stopwatch.GetTimestamp();

            if (!this.isLastTimeValid)
            {
                this.Reset();
            }
            else
            {
                this.actualElapsedTime += CounterToTimeSpan(time - this.lastTime);
                if (!this.IsPaused)
                {
                    this.elapsedTime += CounterToTimeSpan((time - this.lastTime) * this.TimeModifier);
                }
            }

            this.lastTime = time;
            this.isLastTimeValid = true;
        }
    }
}
