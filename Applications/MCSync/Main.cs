﻿namespace CarbonCore.Applications.MCSync
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Compat.Json;
    using CarbonCore.UtilsCommandLine.Contracts;
    
    using MCSync.Contracts;

    using Newtonsoft.Json;

    public class Main : IMain
    {
        private const string SyncProgram = "blinksync.exe";

        private const string VersionIndiciator = ".version";
        
        private static readonly CarbonDirectory SyncRoot = new CarbonDirectory(Assembly.GetEntryAssembly().Location);

        private readonly IList<string> commonSyncObjects = new List<string> { "config", "mods" };

        private readonly IDictionary<string, string> clientOnlySyncObjects = new Dictionary<string, string>
                                                                                 {
                                                                                     { "mods_client", "mods" },
                                                                                     { "resourcepacks", "resourcepacks" },
                                                                                 };

        private readonly IList<string> clientOnlyCopyObjects = new List<string> { "servers.dat" };

        private readonly ICommandLineArguments arguments;

        private CarbonDirectory sourcePath;
        private CarbonDirectory targetPath;

        private bool isServerMode;

        private bool forceSync;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Main(IFactory factory)
        {
            this.arguments = factory.Resolve<ICommandLineArguments>();
            
            this.RegisterCommandLineArguments();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Sync()
        {
            if (!this.arguments.ParseCommandLineArguments())
            {
                // Todo: print usage
                return;
            }

            if (this.CheckVersion())
            {
                Trace.TraceInformation("Skipping sync, no changes found");
                return;
            }
            
            foreach (string syncObject in this.commonSyncObjects)
            {
                using (new ProfileRegion(string.Format("Syncing {0}", syncObject)))
                {
                    this.DoSync(syncObject, syncObject);
                }
            }

            if (!this.isServerMode)
            {
                foreach (string syncSource in this.clientOnlySyncObjects.Keys)
                {
                    using (new ProfileRegion(string.Format("Syncing {0}", syncSource)))
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
            
            Profiler.TraceProfilerStatistics();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool CheckVersion()
        {
            SyncSettings defaults = new SyncSettings();

            CarbonFile sourceVersion = this.sourcePath.ToFile(VersionIndiciator);
            CarbonFile targetVersion = this.targetPath.ToFile(VersionIndiciator);
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
            var proc = new Process
            {
                StartInfo =
                {
                    FileName = SyncRoot.ToFile(SyncProgram).GetPath(),
                    Arguments =
                        string.Format(
                            @"{0} ""{1}\{2}"" ""{3}\{4}""",
                            deleteTargetMismatches ? "-d" : string.Empty,
                            this.sourcePath.GetPath(),
                            syncObject,
                            this.targetPath.GetPath(),
                            syncTarget),
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            proc.Start();
            while (!proc.HasExited)
            {
                this.TraceProcessOutput(proc);

                Thread.Sleep(10);
            }

            this.TraceProcessOutput(proc);
        }

        private void TraceProcessOutput(Process proc)
        {
            string stdOutTemp = proc.StandardOutput.ReadToEnd();
            string stdErrorTemp = proc.StandardError.ReadToEnd();
            Console.WriteLine(stdOutTemp);
            Console.WriteLine(stdErrorTemp);
            Trace.TraceInformation(stdOutTemp);
            if (!string.IsNullOrEmpty(stdErrorTemp))
            {
                Trace.TraceError(stdErrorTemp);
            }
        }

        private void RegisterCommandLineArguments()
        {
            ICommandLineSwitchDefinition definition = this.arguments.Define("s", "sourcePath", x => this.sourcePath = new CarbonDirectory(x));
            definition.RequireArgument = true;
            definition.Description = "The path where the data is located";

            definition = this.arguments.Define("t", "targetPath", x => this.targetPath = new CarbonDirectory(x));
            definition.RequireArgument = true;

            definition = this.arguments.Define("server", x => this.isServerMode = true);
            definition.Description = "Sync as server mode, default is false";

            definition = this.arguments.Define("force", x => this.forceSync = true);
            definition.Description = "Force the sync even if no indicator is present";
        }
    }
}
