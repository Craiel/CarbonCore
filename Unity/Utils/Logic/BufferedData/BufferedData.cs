namespace CarbonCore.Utils.Unity.Logic.BufferedData
{
    using System;
    using System.Threading;

    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Threading;
    using CarbonCore.Utils.Unity.Contracts.BufferedData;

    public abstract class BufferedData : EngineComponent, IBufferedData
    {
        private const bool DefaultAutoCommitEnabled = true;

        private const int DefaultBufferCount = 2;

        private const int DefaultCommitFrameMinThreshold = 10;

        private const int DefaultCommitFrameMaxThreshold = 60;

        private const int DefaultCommitCommandThreshold = 1;

        private readonly IBufferedDataPool pool;

        private readonly ManualResetEvent updateResetEvent;
        
        private readonly int bufferCount;
        
        private long lastCommitFrame;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected BufferedData(IFactory factory, int bufferCount = DefaultBufferCount)
        {
            this.pool = factory.Resolve<IBufferedDataPool>();

            this.updateResetEvent = new ManualResetEvent(false);

            this.bufferCount = bufferCount;

            // Set default settings for the auto buffer commit
            this.AutoCommitEnabled = DefaultAutoCommitEnabled;
            this.CommitFrameMinThreshold = DefaultCommitFrameMinThreshold;
            this.CommitFrameMaxThreshold = DefaultCommitFrameMaxThreshold;
            this.CommitCommandThreshold = DefaultCommitCommandThreshold;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool AutoCommitEnabled { get; protected set; }

        public int CommitFrameMaxThreshold { get; protected set; }

        public int CommitFrameMinThreshold { get; protected set; }

        public int CommitCommandThreshold { get; protected set; }

        public long CurrentCommand
        {
            get
            {
                return this.pool.CurrentCommand;
            }
        }

        public long LatestCommand
        {
            get
            {
                return this.pool.LatestCommand;
            }
        }

        public int CommandErrors
        {
            get
            {
                return this.pool.CommandErrors;
            }
        }

        public override void Initialize()
        {
            var poolSettings = new BufferedDataPoolSettings
                                   {
                                       MaxDatasetCount = Math.Max(DefaultBufferCount, this.bufferCount)
                                   };

            this.pool.Initialize(poolSettings);

            base.Initialize();
        }
        
        public override void Update(EngineTime time)
        {
            this.updateResetEvent.Set();

            base.Update(time);

            if (this.AutoCommitEnabled)
            {
                this.UpdateBufferedDataState(time);
            }

            this.pool.Update();
        }

        public DataSnapshot GetData()
        {
            return this.pool.GetData();
        }

        public long Enqueue(IBufferedDataCommand command)
        {
            return this.pool.Enqueue(command);
        }

        public void Commit()
        {
            this.pool.Commit();
        }

        public void Reset()
        {
            this.pool.Reset();
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateBufferedDataState(EngineTime time)
        {
            // We check if it's time to commit based on certain rules and settings
            if (!this.AttemptBufferCommit(time))
            {
                return;
            }

            this.lastCommitFrame = time.Frame;
            this.Commit();
        }

        private bool AttemptBufferCommit(EngineTime time)
        {
            // Command count first
            if (this.LatestCommand - this.CurrentCommand >= this.CommitCommandThreshold)
            {
                if (UnityConstants.EnableVerboseLogging)
                {
                    Diagnostic.Info(this.GetType().Name + " Commit, Reason: Pending Commands");
                }

                return true;
            }

            // Frame count second
            Diagnostic.Assert(this.CommitFrameMaxThreshold > this.CommitFrameMinThreshold);

            long elapsedFrames = time.Frame - this.lastCommitFrame;
            if (elapsedFrames < this.CommitFrameMinThreshold)
            {
                // It's too soon to commit again
                return false;
            }

            if (elapsedFrames > this.CommitFrameMaxThreshold)
            {
                if (UnityConstants.EnableVerboseLogging)
                {
                    Diagnostic.Info(this.GetType().Name + " Commit, Reason: Frame Time");
                }

                return true;
            }

            return false;
        }
    }
}
