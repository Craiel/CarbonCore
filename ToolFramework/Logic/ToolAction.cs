namespace CarbonCore.ToolFramework.Logic
{
    using System;
    using System.Threading;
    using System.Windows.Threading;

    using CarbonCore.ToolFramework.Contracts;

    public abstract class ToolAction : IToolAction
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ToolActionLevel Level { get; set; }

        public int Order { get; set; }
        
        public bool CanCancel { get; set; }

        public int Progress { get; set; }

        public int ProgressMax { get; set; }

        public string ProgressMessage { get; set; }

        public IToolActionResult Result { get; set; }

        public Dispatcher Dispatcher { get; set; }

        public bool IsRunning { get; protected set; }

        public abstract void Execute(CancellationToken token);

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
