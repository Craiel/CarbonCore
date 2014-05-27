namespace CarbonCore.Utils.Threading
{
    using System;

    using CarbonCore.Utils.Contracts;

    public class ThreadQueueOperation : IThreadQueueOperation
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ThreadQueueOperation(Func<IThreadQueueOperationPayload, bool> action, TimeSpan queueTime)
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

        public TimeSpan QueueTime { get; private set; }

        public TimeSpan ExecutionTime { get; set; }
    }
}
