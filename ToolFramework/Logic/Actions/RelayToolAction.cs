namespace CarbonCore.ToolFramework.Logic.Actions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using CarbonCore.ToolFramework.Contracts;

    public class RelayToolAction : ToolAction
    {
        private readonly Action<IToolAction, CancellationToken> action;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public RelayToolAction(Action<IToolAction, CancellationToken> action)
        {
            this.action = action;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IToolAction Create(Action<IToolAction, CancellationToken> target)
        {
            return new RelayToolAction(target);
        }

        public override void Execute(CancellationToken token)
        {
            this.IsRunning = true;
            Task.Factory.StartNew(() => this.DoExecute(token));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoExecute(CancellationToken token)
        {
            if (this.Dispatcher != null)
            {
                this.Dispatcher.Invoke(() => this.action(this, token));
            }
            else
            {
                var task = Task.Factory.StartNew(() => this.action(this, token));
                Task.WaitAny(task);
            }

            this.IsRunning = false;
        }
    }
}
