namespace CarbonCore.Utils.Threading
{
    using System;

    using CarbonCore.Utils.Contracts;

    public class ThreadQueueOperation : IThreadQueueOperation
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ThreadQueueOperation(Func<IThreadQueueOperationPayload, bool> action, long queueTime)
        {
            this.Action = action;
            this.QueueTime = queueTime;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool Suceeded { get; set; }

        public Func<IThreadQueueOperationPayload, bool> Action { get; private set; }

        public IThreadQueueOperationPayload Payload { get; set; }

        public long QueueTime { get; private set; }

        public long ExecutionTime { get; set; }
    }
}
