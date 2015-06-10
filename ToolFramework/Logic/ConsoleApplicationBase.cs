namespace CarbonCore.ToolFramework.Logic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Threading;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Logic.Actions;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts.IoC;

    public abstract class ConsoleApplicationBase : IConsoleApplicationBase
    {
        private readonly IFactory factory;

        private Dispatcher mainDispatcher;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ConsoleApplicationBase(IFactory factory)
        {
            this.factory = factory;

            // Configure log4net
            log4net.Config.XmlConfigurator.Configure();

            this.Version = AssemblyExtensions.GetVersion(this.GetType());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Name { get; }
        
        public virtual Version Version { get; private set; }

        public virtual void Start()
        {
            Application application = Application.Current ?? new Application();

            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            IList<IToolAction> startupActions = new List<IToolAction>();

            startupActions.Add(RelayToolAction.Create(this.StartupInitializeLogic));

            this.StartupInitializeCustomActions(startupActions);
            
            IToolAction finalAction = RelayToolAction.Create(this.StartupFinalize);
            finalAction.Dispatcher = application.Dispatcher;
            finalAction.Order = int.MaxValue;
            startupActions.Add(finalAction);

            this.mainDispatcher = Dispatcher.CurrentDispatcher;

            // Run the startup actions in another thread
            Task.Factory.StartNew(() => this.ExecuteActions(startupActions));
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void StartFinished();

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual void StartupInitializeLogic(IToolAction toolAction, CancellationToken cancellationToken)
        {
            using (new ToolActionRegion(this.factory, toolAction))
            {
            }
        }
        
        protected virtual void StartupInitializeCustomActions(IList<IToolAction> target)
        {
            // Used by inherited classes to add actions
        }

        protected void StartupFinalize(IToolAction toolAction, CancellationToken cancellationToken)
        {
            // Invoke back into the main thread
            this.mainDispatcher.Invoke(this.StartFinished);
        }

        protected virtual void OnMainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected virtual void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
        }

        private void ExecuteActions(IList<IToolAction> startupActions)
        {
            CancellationToken token = new CancellationToken();
            foreach (IToolAction action in startupActions)
            {
                action.Execute(token);
                while (action.IsRunning)
                {
                    Thread.Sleep(10);
                }
            }
        }
    }
}
