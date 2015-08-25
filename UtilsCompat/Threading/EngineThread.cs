namespace CarbonCore.Utils.Compat.Threading
{
    using System;
    using System.Diagnostics;
    using System.Threading;

    using CarbonCore.Utils.Compat.Diagnostics;

    public delegate bool EngineThreadUpdateDelegate(EngineTime time);

    public class EngineThread
    {
        private const int DefaultShutdownTimeout = 100000;

        private static readonly long PerformanceMeasureInterval = Stopwatch.Frequency;   

        private static int nextThreadId;

        private readonly ManualResetEvent shutdownEvent;

        private readonly EngineThreadUpdateDelegate threadAction;
        
        private readonly EngineTime time;

        private readonly object synchronizationObject = new object();

        private readonly bool useFrameDelay;

        private readonly int targetFrameRate;

        private readonly EngineThreadSettings settings;

        private readonly long frameDelayTargetOptimal;

        private readonly int framesUntilMeasure;

        private volatile bool isRunning;

        private Thread internalThread;

        private long frameDelay;
        private long frameDelayTarget;

        private int framesSinceMeasure;

        private long deltaSinceMeasure;

        private long framesSincePerformanceMeasure;
        private long deltaSincePerformanceMeasure;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public EngineThread(EngineThreadUpdateDelegate threadMain, string name, EngineThreadSettings settings = null)
        {
            this.settings = settings ?? new EngineThreadSettings();

            this.threadAction = threadMain;
            if (this.settings.ThrottleFrameRate)
            {
                this.useFrameDelay = true;
                this.targetFrameRate = this.settings.TargetFrameRate;
                this.frameDelayTargetOptimal = Stopwatch.Frequency / this.targetFrameRate;
                this.frameDelayTarget = this.frameDelayTargetOptimal;
            }

            this.time = new EngineTime();

            // We start the first delay measure after 1s
            this.framesUntilMeasure = 10;

            this.shutdownEvent = new ManualResetEvent(false);
            this.ThreadId = Interlocked.Increment(ref nextThreadId);
            this.ThreadName = name ?? string.Format("Engine Thread {0}", this.ThreadId);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int ThreadId { get; private set; }

        public int ManagedThreadId
        {
            get
            {
                if (this.internalThread != null)
                {
                    return this.internalThread.ManagedThreadId;
                }

                return -1;
            }
        }

        public string ThreadName { get; private set; }

        public bool IsThreadRunning { get; private set; }

        public bool IsThreadFinished { get; private set; }

        public bool MuteTrace { get; set; }

        public void Shutdown()
        {
            this.isRunning = false;

            if (!this.shutdownEvent.WaitOne(DefaultShutdownTimeout))
            {
                throw new Exception(string.Format("Thread {0} did not shut down in time", this.ThreadName));
            }

            this.internalThread = null;
        }

        public void Start()
        {
            if (this.IsThreadRunning || this.IsThreadFinished)
            {
                throw new InvalidOperationException(string.Format("Thread {0} is already running or has finished!", this.ThreadName));
            }

            this.IsThreadRunning = true;
            this.isRunning = true;
            this.internalThread = new Thread(this.ThreadMain) { Name = this.ThreadName, IsBackground = true };
            this.internalThread.Start();

            if (this.MuteTrace)
            {
                Diagnostic.SetMute(this.internalThread.ManagedThreadId);
            }
        }

        public void Synchronize(Action callback)
        {
            lock (this.synchronizationObject)
            {
                callback();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ThreadMain()
        {
            Diagnostic.RegisterThread(this.ThreadName, this.time);

            while (this.isRunning)
            {
                this.time.Update();

                if (!this.useFrameDelay || this.CheckThreadDelay())
                {
                    // Set the new frame
                    this.time.UpdateFrame();

                    this.CheckThreadPerformance();

                    using (new ProfileRegion("EngineThread.Update: " + this.ThreadName) { IncludeThreadId = true })
                    {
                        try
                        {
                            if (!this.threadAction(this.time))
                            {
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            this.isRunning = false;
                            Diagnostic.Exception(e);
                        }
                    }
                }
            }

            this.IsThreadRunning = false;
            this.IsThreadFinished = true;
            this.shutdownEvent.Set();

            Diagnostic.Warning("Engine Thread {0} ({1}) Ended", this.ThreadId, this.ThreadName);
            Diagnostic.UnregisterThread();
        }

        private bool CheckThreadDelay()
        {
            this.frameDelay += this.time.TickDeltaTicks;
            this.deltaSinceMeasure += this.time.TickDeltaTicks;

            if (this.frameDelay < this.frameDelayTarget)
            {
                Thread.Sleep(1);
                return false;
            }

            this.frameDelay = 0;
            this.framesSinceMeasure++;

            if (this.framesSinceMeasure >= this.framesUntilMeasure)
            {
                float current = this.deltaSinceMeasure / (float)this.framesSinceMeasure;
                float percentage = this.frameDelayTargetOptimal / current;

                this.frameDelayTarget = (long)(this.frameDelayTarget * percentage);
                this.framesSinceMeasure = 0;
                this.deltaSinceMeasure = 0;
            }

            return true;
        }

        private void CheckThreadPerformance()
        {
            this.deltaSincePerformanceMeasure += this.time.FrameDeltaTicks;
            this.framesSincePerformanceMeasure++;

            if (this.deltaSincePerformanceMeasure < PerformanceMeasureInterval)
            {
                return;
            }

            Diagnostic.Info("Thread: {0} @ {1}fps", this.ThreadName, this.framesSincePerformanceMeasure);

            this.framesSincePerformanceMeasure = 0;
            this.deltaSincePerformanceMeasure = 0;
        }
    }
}
