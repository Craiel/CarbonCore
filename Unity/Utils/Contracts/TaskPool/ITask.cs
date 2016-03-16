namespace CarbonCore.Unity.Utils.Contracts.TaskPool
{
    using System;

    using CarbonCore.Unity.Utils.Logic.TaskPoolLogic;

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
