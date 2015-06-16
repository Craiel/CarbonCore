namespace CarbonCore.ToolFramework.Logic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.ToolFramework.Logic.Actions;
    using CarbonCore.ToolFramework.View;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Compat.Contracts.IoC;

    public abstract class WindowApplicationBase : IWindowApplicationBase
    {
        private readonly IFactory factory;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected WindowApplicationBase(IFactory factory)
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

        public Window MainWindow { get; private set; }

        public virtual Version Version { get; private set; }

        public virtual void Start()
        {
            Application application = Application.Current;
            bool applicationDispatcherRunning = true;
            if (application == null)
            {
                application = new Application();
                applicationDispatcherRunning = false;

            }
            application.ShutdownMode = ShutdownMode.OnExplicitShutdown;

            IList<IToolAction> startupActions = new List<IToolAction>();

            startupActions.Add(RelayToolAction.Create(this.StartupInitializeLogic));

            this.StartupInitializeCustomActions(startupActions);

            IToolAction action = RelayToolAction.Create(this.StartupInitializeWindow);
            action.Dispatcher = application.Dispatcher;
            startupActions.Add(action);

            IToolAction finalAction = RelayToolAction.Create(this.StartupFinalize);
            finalAction.Dispatcher = application.Dispatcher;
            finalAction.Order = int.MaxValue;
            startupActions.Add(finalAction);

            try
            {
                application.MainWindow = ToolActionDialog.CreateNew(this.factory, startupActions, ToolActionDisplayMode.Splash);
                application.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                application.MainWindow.Title = string.Format("Preparing {0}...", this.Name);
                if (applicationDispatcherRunning)
                {
                    application.MainWindow.Show();
                }
                else
                {
                    application.Run(application.MainWindow);  
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError("Unhandled exception in Application run: {0}", e);
                application.Shutdown(-1);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected Type MainWindowType { get; set; }

        protected object DataContext { get; set; }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            if (this.MainWindow != null)
            {
                this.MainWindow.Closing -= this.OnMainWindowClosing;
                this.MainWindow.Closed -= this.OnMainWindowClosed;
                this.MainWindow = null;
            }
        }

        protected virtual void StartupInitializeLogic(IToolAction toolAction, CancellationToken cancellationToken)
        {
            System.Diagnostics.Trace.TraceWarning("Test");
            using (new ToolActionRegion(this.factory, toolAction))
            {
            }
        }

        protected virtual void StartupInitializeWindow(IToolAction toolAction, CancellationToken cancellationToken)
        {
            using (new ToolActionRegion(this.factory, toolAction))
            {
                this.MainWindow = (Window)Activator.CreateInstance(this.MainWindowType);
                this.MainWindow.Closing += this.OnMainWindowClosing;
                this.MainWindow.Closed += this.OnMainWindowClosed;
                this.MainWindow.DataContext = this.DataContext;
            }
        }

        protected virtual void StartupInitializeCustomActions(IList<IToolAction> target)
        {
            // Used by inherited classes to add actions
        }

        protected void StartupFinalize(IToolAction toolAction, CancellationToken cancellationToken)
        {
            using (new ToolActionRegion(this.factory, toolAction))
            {
                if (Application.Current.MainWindow != null)
                {
                    Application.Current.MainWindow.Close();
                }

                Application.Current.MainWindow = this.MainWindow;
                Application.Current.MainWindow.Show();
            }
        }

        protected virtual void OnMainWindowClosed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        protected virtual void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
        }
    }
}
