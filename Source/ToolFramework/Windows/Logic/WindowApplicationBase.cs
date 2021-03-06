﻿namespace CarbonCore.ToolFramework.Windows.Logic
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Threading;
    using System.Windows;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Logic;
    using CarbonCore.ToolFramework.Logic.Actions;
    using CarbonCore.ToolFramework.Windows.Contracts;
    using CarbonCore.ToolFramework.Windows.Contracts.ViewModels;
    using CarbonCore.ToolFramework.Windows.Logic.Actions;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts.IoC;

    using NLog;

    using ToolActionDialog = CarbonCore.ToolFramework.Windows.View.ToolActionDialog;

    public abstract class WindowApplicationBase : IWindowApplicationBase
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFactory factory;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected WindowApplicationBase(IFactory factory)
        {
            this.factory = factory;
            
            this.Version = AssemblyExtensions.GetVersion(this.GetType());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Name { get; }

        public Window MainWindow { get; private set; }

        public IBaseViewModel MainViewModel { get; private set; }

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

            IToolAction action = DispatcherToolAction.Create(this.StartupInitializeViewModel);
            startupActions.Add(action);

            action = DispatcherToolAction.Create(this.StartupInitializeWindow);
            startupActions.Add(action);

            IToolAction finalAction = DispatcherToolAction.Create(this.StartupFinalize);
            finalAction.Order = int.MaxValue;
            startupActions.Add(finalAction);

            try
            {
                application.MainWindow = ToolActionDialog.CreateNew(this.factory, startupActions, ToolActionDisplayMode.Splash);
                application.MainWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
                application.MainWindow.Title = $"Preparing {this.Name}...";
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
                Logger.Error("Unhandled exception in Application run: {0}", e);
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
        }
        
        protected virtual void StartupInitializeCustomActions(IList<IToolAction> target)
        {
            // Used by inherited classes to add actions
        }

        protected void StartupFinalize(IToolAction toolAction)
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

        protected virtual Window DoInitializeMainWindow()
        {
            throw new InvalidOperationException("MainWindow Initialization must be implemented");
        }

        protected virtual IBaseViewModel DoInitializeMainViewModel()
        {
            Logger.Warn("Main Window Initialization is not implemented, Window will not have DataContext!");
            return null;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void StartupInitializeViewModel(IToolAction toolAction)
        {
            this.MainViewModel = this.DoInitializeMainViewModel();
            if (this.MainWindow != null)
            {
                this.MainViewModel.Initialize();
                this.MainWindow.DataContext = this.MainViewModel;
            }
        }

        private void StartupInitializeWindow(IToolAction toolAction)
        {
            using (new ToolActionRegion(this.factory, toolAction))
            {
                this.MainWindow = this.DoInitializeMainWindow();
                this.MainWindow.Closing += this.OnMainWindowClosing;
                this.MainWindow.Closed += this.OnMainWindowClosed;
                if (this.MainViewModel != null)
                {
                    this.MainWindow.DataContext = this.MainViewModel;
                }
            }
        }
    }
}
