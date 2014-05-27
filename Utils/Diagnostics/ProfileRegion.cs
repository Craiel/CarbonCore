namespace CarbonCore.Utils.Diagnostics
{
    using System;
    using System.Diagnostics;

    public class ProfileRegion : IDisposable
    {
        private readonly Stopwatch timer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ProfileRegion(string name)
        {
            this.Name = name;

            Profiler.RegionStart(this);

            this.timer = new Stopwatch();
            this.timer.Start();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long ElapsedMilliseconds
        {
            get
            {
                return this.timer.ElapsedMilliseconds;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                return this.timer.ElapsedTicks;
            }
        }

        public string Name { get; private set; }

        public object Tag { get; set; }

        public void Dispose()
        {
            this.Dispose(false);
        }

        public void Dispose(bool discard)
        {
            this.timer.Stop();

            if (!discard)
            {
                Profiler.RegionFinish(this);
            }

            GC.SuppressFinalize(this);
        }
    }
}
