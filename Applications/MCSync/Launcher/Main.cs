namespace CarbonCore.Applications.MCSync.Launcher
{
    using System.Diagnostics;
    using System.Text;

    using CarbonCore.Applications.MCSync.Launcher.Contracts;
    using CarbonCore.ToolFramework.Console.Logic;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    public class Main : ConsoleApplicationBase, IMain
    {
        private const string MCSyncExecutable = "MCSync.exe";

        private const string ConfigFileName = "MCSync.Launcher.config";

        private readonly IConfig config;

        private CarbonFile configFile;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Main(IFactory factory) : base(factory)
        {
            this.config = factory.Resolve<IConfig>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name => "MCSync.Launcher";

        protected override void StartFinished()
        {
            this.configFile = new CarbonFile(ConfigFileName);
            this.config.Load(this.configFile);

            if (this.config.Current.Root == null || !this.config.Current.Root.Exists)
            {
                Diagnostic.Error("Root directory is invalid or does not exist");
                return;
            }

            string source = this.config.Current.SourcePath;
            string target = this.config.Current.TargetPath;
            bool force = this.config.Current.Force;
            bool server = this.config.Current.Server;

            if (string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target))
            {
                Diagnostic.Error("Invalid source or target path");
                return;
            }

            CarbonFile syncExecutable = this.config.Current.Root.ToFile(MCSyncExecutable);
            if (!syncExecutable.Exists)
            {
                Diagnostic.Error("Could not locate MCSync executable");
                return;
            }

            StringBuilder arguments = new StringBuilder();
            arguments.AppendFormat("-s \"{0}\" -t \"{1}\"", source, target);

            if (force)
            {
                arguments.Append(" -force");
            }

            if (server)
            {
                arguments.Append(" -server");
            }
            
            using (var syncProcess = new Process())
            {
                syncProcess.StartInfo = new ProcessStartInfo(syncExecutable.GetPath(), arguments.ToString()) { UseShellExecute = false };
                syncProcess.Start();
                syncProcess.WaitForExit();
            }
        }
    }
}
