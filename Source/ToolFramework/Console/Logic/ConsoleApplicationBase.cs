namespace CarbonCore.ToolFramework.Console.Logic
{
    using System;
    using System.ComponentModel;

    using CarbonCore.ToolFramework.Console.Contracts;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Edge.CommandLine.Contracts;
    using CarbonCore.Utils.I18N;

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
        
        public virtual Version Version { get; }

        public virtual void Start()
        {
            // Set the default locale to english
            Localization.CurrentCulture = LocaleConstants.LocaleEnglishUS;

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

            Localization.SaveDictionaries();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected ICommandLineArguments Arguments { get; }

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
                return false;
            }

            return true;
        }
        
        protected virtual void OnMainWindowClosed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        protected virtual void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
        }
    }
}
