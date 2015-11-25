namespace CarbonCore.Utils.Unity.Contracts.TaskPool
{
    using System;

    using CarbonCore.Utils.Unity.Contracts;
    using CarbonCore.Utils.Unity.Logic.TaskPoolLogic;

    public interface ITaskPool : IEngineComponent, IDisposable
    {
        void Start(Task task);

        void Reset(int processorCount);
    }
}
