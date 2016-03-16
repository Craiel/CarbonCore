namespace CarbonCore.Unity.Utils.Logic.BufferedData
{
    using System;
    using System.Linq;

    using CarbonCore.Unity.Utils.Contracts.BufferedData;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;

    public class BufferedDataPool : IBufferedDataPool
    {
        private readonly IFactory factory;

        private readonly BufferedDataCommandQueue commandQueue;

        private readonly object commitLock;

        private BufferedDataPoolSettings settings;

        private IBufferedDataSetInternal[] datasets;

        private bool[] datasetAvailabilityState;
        private long[] datasetTransactionState;

        private IBufferedDataSetInternal[] dedicatedDatasets;

        private long[] dedicatedDatasetTransactionState;

        private IBufferedDataSetInternal dynamicMasterDataset;

        private long dynamicMasterTransactionState;

        private int activeDataset;

        private int datasetCount;

        private int dedicatedDatasetCount;
        
        private bool commitPending;

        private long currentCommand;

        private long latestCommand;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferedDataPool(IFactory factory)
        {
            this.factory = factory;
            this.commandQueue = new BufferedDataCommandQueue();

            this.commitLock = new object();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int ActiveDataset
        {
            get
            {
                return this.activeDataset;
            }
        }

        public int DatasetCount
        {
            get
            {
                return this.datasetCount;
            }
        }
        
        public int DedicatedDatasetCount
        {
            get
            {
                return this.dedicatedDatasetCount;
            }
        }
        
        public long CurrentCommand
        {
            get
            {
                return this.currentCommand;
            }
        }

        public long LatestCommand
        {
            get
            {
                return this.latestCommand;
            }
        }

        public int CommandErrors { get; private set; }

        public void Initialize(BufferedDataPoolSettings setting)
        {
            this.settings = setting;

            // Initialize the dynamic / static datasets
            this.datasetCount = this.settings.UseDynamicDatasetAllocation
                ? this.settings.MinDatasetCount 
                : this.settings.MaxDatasetCount;

            this.datasets = new IBufferedDataSetInternal[this.datasetCount];
            this.datasetTransactionState = new long[this.datasetCount];
            this.datasetAvailabilityState = new bool[this.datasetCount];
            for (var i = 0; i < this.datasetCount; i++)
            {
                var dataset = this.factory.Resolve<IBufferedDataSetInternal>();
                dataset.Initialize();
                this.datasets[i] = dataset;
                this.datasetTransactionState[i] = 0;
                this.datasetAvailabilityState[i] = true;
            }

            // Initialize the dedicated datasets
            this.dedicatedDatasetCount = this.settings.DedicatedDatasets;
            if (this.dedicatedDatasetCount > 0)
            {
                this.dedicatedDatasets = new IBufferedDataSetInternal[this.dedicatedDatasetCount];
                this.dedicatedDatasetTransactionState = new long[this.dedicatedDatasetCount];
                for (var i = 0; i < this.dedicatedDatasetCount; i++)
                {
                    var dataset = this.factory.Resolve<IBufferedDataSetInternal>();
                    dataset.Initialize();
                    this.dedicatedDatasets[i] = dataset;
                    this.dedicatedDatasetTransactionState[i] = 0;
                }
            }

            if (this.settings.UseDynamicDatasetAllocation)
            {
                this.dynamicMasterDataset = this.factory.Resolve<IBufferedDataSetInternal>();
                this.dynamicMasterDataset.Initialize();
                this.dynamicMasterTransactionState = 0;
            }
        }
        
        public void Update()
        {
            int availableDatasets = this.UpdateAvailableDatasets();
            if (this.settings.UseDynamicDatasetAllocation)
            {
                if (availableDatasets <= 0)
                {
                    if (this.datasetCount >= this.settings.MaxDatasetCount)
                    {
                        throw new InvalidOperationException("Could not allocate more dataset, max count exceeded");
                    }

                    // Allocate a new dataset instance by cloning the master
                    this.AddDynamicDatasetInstances(1);
                }
            }

            using (new ProfileRegion("BufferedDataPool.UpdateDatasetCommands()") { IncludeThreadId = true })
            {
                this.UpdateDatasetCommands();
            }

            // If we have a pending commit advance the active data to the next buffer
            if (this.commitPending)
            {
                using (new ProfileRegion("BufferedDataPool.UpdateDatasetCommit()") { IncludeThreadId = true })
                {
                    this.UpdateDatasetCommit();
                }
            }
        }

        public DataSnapshot GetDedicatedData(int id = 0)
        {
            lock (this.commitLock)
            {
                return new DataSnapshot(this.dedicatedDatasets[id]);
            }
        }

        public DataSnapshot GetData()
        {
            lock (this.commitLock)
            {
                return new DataSnapshot(this.datasets[this.activeDataset]);
            }
        }

        public long Enqueue(IBufferedDataCommand command)
        {
            // We queue this up in a local command buffer helper structure
            // the command will be transferred into the data instances on update tick
            //Diagnostic.Info("{0} -> Comm: {1}", this.GetType().Name, command.GetType().Name);
            return this.commandQueue.Enqueue(command);
        }

        public void Commit()
        {
            this.commitPending = true;
        }

        public void Reset()
        {
            this.commandQueue.Clear();
            this.CommandErrors = 0;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private int UpdateAvailableDatasets()
        {
            int result = 0;
            for (var i = 0; i < this.datasetCount; i++)
            {
                bool state = this.GetDatasetAvailability(i);
                this.datasetAvailabilityState[i] = state;
                if (state)
                {
                    result++;
                }
            }

            return result;
        }

        private bool GetDatasetAvailability(int index)
        {
            if (index == this.activeDataset)
            {
                return false;
            }

            // Check if anyone is still using this snapshot
            if (this.datasets[index].RefCount() > 0)
            {
                return false;
            }

            return true;
        }

        private void UpdateDatasetCommands()
        {
            // This will track the oldest command that we need to keep
            long lastCommandId = this.commandQueue.LatestId;

            // Update all dataset instances
            for (var i = 0; i < this.datasetCount; i++)
            {
                if (!this.datasetAvailabilityState[i])
                {
                    continue;
                }

                long transactionState = this.datasetTransactionState[i];
                this.UpdateDatasetCommands(this.datasets[i], ref transactionState, ref lastCommandId);
                this.datasetTransactionState[i] = transactionState;
            }

            // Update all dedicated dataset instances
            for (var i = 0; i < this.dedicatedDatasetCount; i++)
            {
                // Check if anyone is still using this snapshot
                if (this.dedicatedDatasets[i].RefCount() > 0)
                {
                    throw new InvalidOperationException("Attempted Update of Dedicated Dataset during reference lock!");
                }

                long transactionState = this.dedicatedDatasetTransactionState[i];
                this.UpdateDatasetCommands(this.dedicatedDatasets[i], ref transactionState, ref lastCommandId);
                this.dedicatedDatasetTransactionState[i] = transactionState;
            }

            if (this.settings.UseDynamicDatasetAllocation)
            {
                // Update the master dynamic instance
                this.UpdateDatasetCommands(
                    this.dynamicMasterDataset,
                    ref this.dynamicMasterTransactionState,
                    ref lastCommandId);
            }

            // Check if we can trim some commands that are no longer needed
            long oldestCommandId = this.datasetTransactionState.Min();
            if (oldestCommandId >= 0 && oldestCommandId < long.MaxValue)
            {
                this.commandQueue.Trim(oldestCommandId);
            }

            this.currentCommand = this.datasetTransactionState.Min();
            this.latestCommand = this.commandQueue.LatestId;
        }

        private void UpdateDatasetCommands(IBufferedDataSetInternal dataset, ref long transactionState, ref long lastCommandId)
        {
            if (transactionState >= lastCommandId)
            {
                // Nothing to update in this dataset
                return;
            }

            long commandCount = this.commandQueue.GetPlaybackCount(transactionState);
            if (commandCount <= 0)
            {
                // No commands to push for this dataset
                return;
            }
            
            // We have to update this dataset with the pending commands
            foreach (BufferedDataPendingCommand command in this.commandQueue.Playback(transactionState + 1))
            {
                // Note: this trace slows things down a lot, only use when needed
                //Diagnostic.Info("{0} Command: {1}", dataset.Id, command.Command.GetType().Name);

                if (this.CommandErrors <= 0)
                {
                    try
                    {
                        dataset.Execute(command.Command);
                    }
                    catch (Exception e)
                    {
                        // Log the error and abort whichever thread we are running in
                        Diagnostic.Error(
                            "Failed to execute command {0}({1}) on Dataset {2}",
                            command.Command.GetType().Name,
                            command.Id,
                            dataset.Id);
                        Diagnostic.Exception(e);
                        this.CommandErrors++;
                    }
                }
                else
                {
                    Diagnostic.Warning(
                        "Dataset {0} is in error state, skipping command {1}({2})",
                        dataset.Id,
                        command.Command.GetType().Name,
                        command.Id);
                }

                transactionState = command.Id;
            }
        }
        
        private void UpdateDatasetCommit()
        {
            // Try to find a dataset that we can activate
            int startPosition = this.activeDataset;
            int currentPosition = startPosition + 1;
            while (currentPosition != startPosition)
            {
                // Loop around the dataset index
                if (currentPosition > this.datasets.Length - 1)
                {
                    currentPosition = 0;
                    continue;
                }

                // If the dataset is still in use it's not valid to be activated
                if (this.datasets[currentPosition].RefCount() > 0)
                {
                    if (currentPosition == startPosition)
                    {
                        // We looped back to the current active dataset, stay on this one until the others become free
                        break;
                    }

                    // This Dataset is still in use, skip to the next one
                    currentPosition++;
                    continue;
                }

                // This Dataset is valid
                break;
            }

            if (this.activeDataset == currentPosition)
            {
                Diagnostic.Warning("Could not Commit DataSet! Instances are probably still referenced");
                return;
            }

            // Only lock when we are actually ready to commit to the new DataSet
            lock (this.commitLock)
            {
                //Diagnostic.Info("Committing DataSet {0} -> {1}", this.activeDataset, currentPosition);
                this.activeDataset = currentPosition;
                this.commitPending = false;
            }
        }

        private void AddDynamicDatasetInstances(int count)
        {
            if (count <= 0)
            {
                throw new ArgumentException("Invalid count value", "count");
            }

            int newCount = this.datasetCount + count;
            Array.Resize(ref this.datasets, newCount);
            Array.Resize(ref this.datasetTransactionState, newCount);
            Array.Resize(ref this.datasetAvailabilityState, newCount);

            for (var i = 0; i < count; i++)
            {
                IBufferedDataSetInternal dataset = this.dynamicMasterDataset.Clone();
                this.datasets[this.datasetCount + i] = dataset;
                this.datasetTransactionState[this.datasetCount + i] = this.dynamicMasterTransactionState;
            }

            this.datasetCount = newCount;
        }
    }
}
