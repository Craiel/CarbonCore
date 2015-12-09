namespace CarbonCore.Applications.MCSync
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    using CarbonCore.ToolFramework.Logic;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;
    using CarbonCore.Utils.Json;
    using CarbonCore.Utils.Edge.CommandLine.Contracts;
    
    using MCSync.Contracts;

    using Newtonsoft.Json;

    public class Main : ConsoleApplicationBase, IMain
    {
        private const string SyncProgram = "blinksync.exe";

        private const string VersionIndicator = ".version";
        
        private static readonly CarbonDirectory SyncRoot = new CarbonDirectory(Assembly.GetEntryAssembly().Location);

        private readonly IList<string> commonSyncObjects = new List<string> { "config", "mods" };

        private readonly IDictionary<string, string> clientOnlySyncObjects = new Dictionary<string, string>
                                                                                 {
                                                                                     { "mods_client", "mods" },
                                                                                     { "resourcepacks", "resourcepacks" },
                                                                                 };

        private readonly IList<string> serverOnlyCopyObjects = new List<string> { VersionIndicator };
        private readonly IList<string> clientOnlyCopyObjects = new List<string> { "servers.dat", VersionIndicator };

        private CarbonDirectory sourcePath;
        private CarbonDirectory targetPath;

        private bool isServerMode;

        private bool forceSync;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Main(IFactory factory) : base(factory)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name => "MCSync";

        protected override void StartFinished()
        {
            this.Sync();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Sync()
        {
            if (this.CheckVersion())
            {
                Trace.TraceInformation("Skipping sync, no changes found");
                return;
            }
            
            foreach (string syncObject in this.commonSyncObjects)
            {
                using (new ProfileRegion($"Syncing {syncObject}"))
                {
                    this.DoSync(syncObject, syncObject);
                }
            }

            if (!this.isServerMode)
            {
                foreach (string syncSource in this.clientOnlySyncObjects.Keys)
                {
                    using (new ProfileRegion($"Syncing {syncSource}"))
                    {
                        this.DoSync(syncSource, this.clientOnlySyncObjects[syncSource], false);
                    }
                }

                using (new ProfileRegion("Copying Additional Files"))
                {
                    foreach (string copyObject in this.clientOnlyCopyObjects)
                    {
                        Trace.TraceInformation(" Copying {0}", copyObject);
                        this.sourcePath.ToFile(copyObject).CopyTo(this.targetPath.ToFile(copyObject), true);
                    }
                }
            }
            else
            {
                using (new ProfileRegion("Copying Additional Files"))
                {
                    foreach (string copyObject in this.serverOnlyCopyObjects)
                    {
                        Trace.TraceInformation(" Copying {0}", copyObject);
                        this.sourcePath.ToFile(copyObject).CopyTo(this.targetPath.ToFile(copyObject), true);
                    }
                }
            }
            
            Profiler.TraceProfilerStatistics();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool CheckVersion()
        {
            SyncSettings defaults = new SyncSettings();

            CarbonFile sourceVersion = this.sourcePath.ToFile(VersionIndicator);
            CarbonFile targetVersion = this.targetPath.ToFile(VersionIndicator);
            if (!sourceVersion.Exists)
            {
                JsonExtensions.SaveToFile(sourceVersion, defaults, false, Formatting.Indented);
                return false;
            }

            if (!targetVersion.Exists)
            {
                JsonExtensions.SaveToFile(targetVersion, defaults, false, Formatting.Indented);
                return false;
            }

            if (this.forceSync)
            {
                return false;
            }

            try
            {
                SyncSettings source = JsonExtensions.LoadFromFile<SyncSettings>(sourceVersion, false);
                SyncSettings target = JsonExtensions.LoadFromFile<SyncSettings>(targetVersion, false);

                return source.Version == target.Version;
            }
            catch (Exception e)
            {
                Trace.TraceError("Failed to read version settings: {0}", e);
                return false;
            }
        }

        private void DoSync(string syncObject, string syncTarget, bool deleteTargetMismatches = true)
        {
            var process = new Process
            {
                StartInfo =
                {
                    FileName = SyncRoot.ToFile(SyncProgram).GetPath(),
                    Arguments =
                        $@"{(deleteTargetMismatches ? "-d" : string.Empty)} ""{this.sourcePath.GetPath()}\{syncObject
                        }"" ""{this.targetPath.GetPath()}\{syncTarget}""",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            while (!process.HasExited)
            {
                this.TraceProcessOutput(process);

                Thread.Sleep(10);
            }

            this.TraceProcessOutput(process);
        }

        private void TraceProcessOutput(Process process)
        {
            string stdOutTemp = process.StandardOutput.ReadToEnd();
            string stdErrorTemp = process.StandardError.ReadToEnd();
            Console.WriteLine(stdOutTemp);
            Console.WriteLine(stdErrorTemp);
            Trace.TraceInformation(stdOutTemp);
            if (!string.IsNullOrEmpty(stdErrorTemp))
            {
                Trace.TraceError(stdErrorTemp);
            }
        }

        protected override bool RegisterCommandLineArguments()
        {
            ICommandLineSwitchDefinition definition = this.Arguments.Define("s", "sourcePath", x => this.sourcePath = new CarbonDirectory(x));
            definition.RequireArgument = true;
            definition.Required = true;
            definition.Description = "The path where the data is located";

            definition = this.Arguments.Define("t", "targetPath", x => this.targetPath = new CarbonDirectory(x));
            definition.RequireArgument = true;
            definition.Required = true;

            definition = this.Arguments.Define("server", x => this.isServerMode = true);
            definition.Description = "Sync as server mode, default is false";

            definition = this.Arguments.Define("force", x => this.forceSync = true);
            definition.Description = "Force the sync even if no indicator is present";

            return true;
        }
    }
}
