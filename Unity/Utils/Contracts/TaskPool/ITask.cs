namespace CarbonCore.Utils.Unity.Contracts.TaskPool
{
    using System;

    using CarbonCore.Utils.Unity.Logic.TaskPoolLogic;

    public interface ITask
    {
        long Id { get; }

        int TaskInstanceId { get; }
        int TaskThreadId { get; }
        
        // These are set during execution
        TaskStatus Status { get; }

        Exception Error { get; }
    }
}
