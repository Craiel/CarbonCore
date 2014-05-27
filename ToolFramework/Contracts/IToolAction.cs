namespace CarbonCore.ToolFramework.Contracts
{
    using System;
    using System.Threading;
    using System.Windows.Threading;

    public enum ToolActionLevel
    {
        Default,
        Required,
        Optional,
    }

    public interface IToolAction : IDisposable
    {
        ToolActionLevel Level { get; set; }

        int Order { get; set; }

        bool CanCancel { get; set; }

        bool IsRunning { get; }

        int Progress { get; set; }
        int ProgressMax { get; set; }

        string ProgressMessage { get; set; }

        IToolActionResult Result { get; set; }
        
        Dispatcher Dispatcher { get; set; }
        
        void Execute(CancellationToken token);
    }
}
