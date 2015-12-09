namespace CarbonCore.Utils.Unity.Logic.TaskPoolLogic
{
    using System;
    using System.Linq;

    using CarbonCore.Utils.Unity.Contracts.TaskPool;
    using CarbonCore.Utils.Unity.Logic;

    public class TaskPool : EngineComponent, ITaskPool
    {
        private TaskPoolInstance[] instances;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Start(Task task)
        {
            // For now we do a very simple load balancing based on task count
            TaskPoolInstance instance = this.instances.OrderBy(x => x.Load).First();
            instance.Enqueue(task);
        }

        public void Reset(int instanceCount)
        {
            if (instanceCount <= 0)
            {
                throw new ArgumentException("Invalid Instance Count: " + instanceCount);
            }

            this.DisposeInstances();

            this.instances = new TaskPoolInstance[instanceCount];
            for (var i = 0; i < instanceCount; i++)
            {
                this.instances[i] = new TaskPoolInstance();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (!isDisposing)
            {
                return;
            }

            this.DisposeInstances();
        }

        private void DisposeInstances()
        {
            if (this.instances == null)
            {
                return;
            }

            foreach (TaskPoolInstance instance in this.instances)
            {
                instance.Dispose();
            }

            this.instances = null;
        }
    }
}
