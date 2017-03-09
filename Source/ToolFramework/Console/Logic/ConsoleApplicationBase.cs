namespace CarbonCore.ToolFramework.Console.Logic
{
    using System;

    using CarbonCore.ToolFramework.Console.Contracts;
    using CarbonCore.Utils;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Edge.CommandLine.Contracts;
    using CarbonCore.Utils.I18N;

    public abstract class ConsoleApplicationBase : IConsoleApplicationBase
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ConsoleApplicationBase(IFactory factory)
        {
            this.Version = AssemblyExtensions.GetVersion(this.GetType());

            this.Arguments = factory.Resolve<ICommandLineArguments>();
            
            this.ExitCode = 0;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Name { get; }
        
        public virtual Version Version { get; }

        public int ExitCode { get; protected set; }

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
#pragma warning disable SA1201 // A property should not follow a method
        protected ICommandLineArguments Arguments { get; }
#pragma warning restore SA1201 // A property should not follow a method

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
    }
}
