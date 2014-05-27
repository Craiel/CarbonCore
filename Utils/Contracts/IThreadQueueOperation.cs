namespace CarbonCore.Utils.Contracts
{
    using System;

    public interface IThreadQueueOperationPayload
    {
        object Data { get; set; }
    }

    public interface IThreadQueueOperation
    {
        bool Suceeded { get; set; }

        Func<IThreadQueueOperationPayload, bool> Action { get; }

        IThreadQueueOperationPayload Payload { get; set; }

        TimeSpan QueueTime { get; }
        TimeSpan ExecutionTime { get; set; }
    }
}
