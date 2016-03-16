namespace CarbonCore.ToolFramework.Windows.Logic.Actions
{
    using System;
    using System.Threading;
    using System.Windows.Threading;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Logic;

    public class DispatcherToolAction : ToolAction
    {
        private readonly Dispatcher currentDispatcher;

        private readonly Action<IToolAction> action;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DispatcherToolAction(Action<IToolAction> action)
        {
            this.currentDispatcher = Dispatcher.CurrentDispatcher;
            this.action = action;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IToolAction Create(Action<IToolAction> target)
        {
            return new DispatcherToolAction(target);
        }

        public override void Execute(CancellationToken token)
        {
            this.IsRunning = true;
            this.currentDispatcher.Invoke(() => this.action(this));
            this.IsRunning = false;
        }
    }
}
