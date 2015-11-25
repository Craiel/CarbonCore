namespace CarbonCore.Utils.Unity.Logic.TaskPoolLogic
{
    using System;
    using System.Threading;

    using CarbonCore.Utils.Unity.Contracts.TaskPool;

    public delegate object TaskExecutionDelegate(Task task);
    public delegate void TaskCallbackDelegate(ITask task, object data);

    public class Task : ITask
    {
        private static long nextId;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Task(TaskExecutionDelegate action, TaskCallbackDelegate callback = null, ManualResetEvent resetEvent = null)
        {
            this.Id = Interlocked.Increment(ref nextId);
            this.Action = action;
            this.Callback = callback;
            this.ResetEvent = resetEvent ?? new ManualResetEvent(false);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long Id { get; private set; }

        public int TaskInstanceId { get; set; }
        public int TaskThreadId { get; set; }

        public TaskExecutionDelegate Action { get; private set; }
        public TaskCallbackDelegate Callback { get; private set; }
        public ManualResetEvent ResetEvent { get; private set; }

        public TaskStatus Status { get; set; }

        public Exception Error { get; set; }
    }
}
