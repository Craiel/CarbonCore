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

        private long elapsedTime;
        private long actualElapsedTime;

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

        public long ElapsedTime
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

        public long ActualElapsedTime
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

        public long TimeLostToPause
        {
            get
            {
                return this.ActualElapsedTime - this.ElapsedTime;
            }
        }

        public bool AutoUpdate { get; set; }

        public bool IsPaused { get; set; }

        public static TimeSpan TimeToTimeSpan(long time)
        {
            return TimeSpan.FromTicks((long)(time * 10000000.0f) / Frequency);
        }

        public void Reset()
        {
            this.elapsedTime = 0;
            this.actualElapsedTime = 0;
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
                this.actualElapsedTime += time - this.lastTime;
                if (!this.IsPaused)
                {
                    this.elapsedTime += (long)((time - this.lastTime) * this.TimeModifier);
                }
            }

            this.lastTime = time;
            this.isLastTimeValid = true;
        }
    }
}
