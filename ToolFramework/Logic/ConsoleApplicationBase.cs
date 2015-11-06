namespace CarbonCore.ToolFramework.Logic
{
    using System;
    using System.ComponentModel;
    using System.Windows;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.UtilsCommandLine.Contracts;

    public abstract class ConsoleApplicationBase : IConsoleApplicationBase
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ConsoleApplicationBase(IFactory factory)
        {
            // Configure log4net
            log4net.Config.XmlConfigurator.Configure();

            this.Version = AssemblyExtensions.GetVersion(this.GetType());

            this.Arguments = factory.Resolve<ICommandLineArguments>();

            Diagnostic.RegisterThread(this.GetType().Name);
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

            if (!this.RegisterCommandLineArguments())
            {
                return;
            }

            if (!this.StartupInitializeLogic())
            {
                return;
            }

            if (!this.ParseCommandLineArguments())
            {
                this.Arguments.PrintArgumentUse();
                return;
            }

            this.StartFinished();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected ICommandLineArguments Arguments { get; private set; }

        protected abstract void StartFinished();

        protected virtual void Dispose(bool disposing)
        {
        }

        protected virtual bool RegisterCommandLineArguments()
        {
            return true;
        }

        protected virtual bool StartupInitializeLogic()
        {
            return true;
        }

        protected virtual bool ParseCommandLineArguments()
        {
            if (!this.Arguments.ParseCommandLineArguments())
            {
                this.Arguments.PrintArgumentUse();
                return false;
            }

            return true;
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
