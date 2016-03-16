namespace CarbonCore.Unity.Utils.Contracts.TaskPool
{
    using System;

    using CarbonCore.Unity.Utils.Logic.TaskPoolLogic;

    public interface ITaskPool : IEngineComponent, IDisposable
    {
        void Start(Task task);

        void Reset(int processorCount);
    }
}
