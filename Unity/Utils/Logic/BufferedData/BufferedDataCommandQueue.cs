namespace CarbonCore.Unity.Utils.Logic.BufferedData
{
    using System.Collections.Generic;

    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    public class BufferedDataCommandQueue
    {
        private readonly IDictionary<long, BufferedDataPendingCommand> commands;

        private long nextId;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BufferedDataCommandQueue()
        {
            this.commands = new Dictionary<long, BufferedDataPendingCommand>();

            // This will force it to be updated with the next command
            this.OldestId = long.MaxValue;
        }

        public long OldestId { get; private set; }

        public long LatestId { get; private set; }

        public BufferedDataPendingCommand Get(long id)
        {
            lock (this.commands)
            {
                BufferedDataPendingCommand command;
                if (this.commands.TryGetValue(id, out command))
                {
                    return command;
                }
            }

            return null;
        }

        public void Clear()
        {
            lock (this.commands)
            {
                this.Trim(this.LatestId + 1);
            }
        }

        public long GetPlaybackCount(long startId)
        {
            long id = startId;
            if (id < this.OldestId)
            {
                id = this.OldestId;
            }

            return 1 + (this.LatestId - id);
        }

        public IEnumerable<BufferedDataPendingCommand> Playback(long startId)
        {
            long id = startId;
            if (id < this.OldestId)
            {
                id = this.OldestId;
            }

            while (id <= this.LatestId)
            {
                lock (this.commands)
                {
                    yield return this.commands[id++];
                }
            }
        }

        public void Trim(long targetId)
        {
            if (this.OldestId == long.MaxValue || targetId <= this.OldestId)
            {
                // Ignore invalid state's for a trim
                return;
            }

            // Diagnostic.Info("Trimming {0} Commands from Queue", targetId - this.OldestId);

            long id = this.OldestId;
            while (id < targetId)
            {
                lock (this.commands)
                {
                    this.commands.Remove(id++);
                    this.OldestId = id;
                }
            }
        }

        public long Enqueue(IBufferedDataCommand command)
        {
            lock (this.commands)
            {
                var pendingCommand = new BufferedDataPendingCommand(++this.nextId, command);

                // We rely on this always increasing, no need to check
                long id = pendingCommand.Id;
            
                this.commands.Add(id, pendingCommand);
                this.LatestId = id;
                if (this.OldestId > id)
                {
                    this.OldestId = id;
                }

                return pendingCommand.Id;
            }
        }
    }
}
