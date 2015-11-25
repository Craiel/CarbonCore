namespace CarbonCore.Utils.Unity.Logic.TaskPoolLogic
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using CarbonCore.Utils.Compat.Diagnostics;

    public class TaskPoolInstance : IDisposable
    {
        private static int nextId;
        
        private readonly Queue<Task> queue;

        private readonly ManualResetEvent waitLock;

        private readonly ManualResetEvent disposeLock;

        private readonly int id;

        private bool isActive;

        private volatile int load;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TaskPoolInstance()
        {
            this.queue = new Queue<Task>();

            this.waitLock = new ManualResetEvent(false);
            this.disposeLock = new ManualResetEvent(false);

            this.isActive = true;
            this.id = Interlocked.Increment(ref nextId);

            // Bring up the thread
            var thread = new Thread(this.ThreadMain)
                              {
                                  Name = string.Concat("TaskPoolInstance", this.id),
                                  IsBackground = true
                              };
            thread.Start();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Load
        {
            get
            {
                return this.load;
            }
        }

        public void Enqueue(Task task)
        {
            this.queue.Enqueue(task);
            this.load++;
            this.waitLock.Set();
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

            // Wait for the thread to stop gracefully
            this.isActive = false;
            this.waitLock.Set();
            this.disposeLock.WaitOne();
        }

        private void ThreadMain()
        {
            while (this.isActive)
            {
                int currentCount;
                lock (this.queue)
                {
                    currentCount = this.queue.Count;
                }

                if (this.isActive && currentCount <= 0)
                {
                    this.waitLock.WaitOne();
                    if (!this.isActive)
                    {
                        break;
                    }
                }

                // Fetch the next task
                Task task;
                lock (this.queue)
                {
                    if (this.queue.Count <= 0)
                    {
                        continue;
                    }

                    task = this.queue.Dequeue();
                    this.load--;
                }

                object taskDataResult = null;

                // Execute the task action, we are safeguarding this call
                try
                {
                    task.TaskInstanceId = this.id;
                    task.TaskThreadId = Thread.CurrentThread.ManagedThreadId;
                    taskDataResult = task.Action(task);

                    // We assume that unknown status here means we succeeded since Action did not fail
                    //  might have to remove that assumption if we want to enforce a status set
                    if (task.Status == TaskStatus.Unknown)
                    {
                        task.Status = TaskStatus.Succeeded;
                    }
                }
                catch (Exception e)
                {
                    Diagnostic.Exception(e);
                    task.Error = e;
                    task.Status = TaskStatus.Errored;
                }

                if (task.Callback != null)
                {
                    task.Callback(task, taskDataResult);
                }

                // Trigger the reset event on the task if present
                if (task.ResetEvent != null)
                {
                    task.ResetEvent.Set();
                }
            }

            this.disposeLock.Set();
        }
    }
}
