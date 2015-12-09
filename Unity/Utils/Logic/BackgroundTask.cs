namespace CarbonCore.Utils.Unity.Logic
{
    using System;
    using System.Threading;

    using CarbonCore.Utils.Diagnostics;

    public class BackgroundTask : IDisposable
    {
        private readonly Thread thread;

        private readonly Action threadMain;

        private readonly ManualResetEvent resetEvent;

        private bool isActive;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BackgroundTask(Action threadMain, string name = null)
        {
            this.threadMain = threadMain;

            this.resetEvent = new ManualResetEvent(false);
            this.isActive = true;

            this.thread = new Thread(this.ThreadMain) { Name = name ?? "No Name Background Task", IsBackground = true };
            this.thread.Start();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
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

            this.isActive = false;
            if (this.resetEvent.WaitOne(1000))
            {
                return;
            }

            Diagnostic.Warning("Background Task did not end gracefully!");
            this.thread.Abort();
        }

        private void ThreadMain()
        {
            while (this.isActive)
            {
                this.threadMain();
            }

            this.resetEvent.Set();
        }
    }
}
