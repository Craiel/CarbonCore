namespace MCSync
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Threading;

    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.UtilsCommandLine.Contracts;
    
    using MCSync.Contracts;
    
    public class Main : IMain
    {
        private const string SyncProgram = "blinksync.exe";
        
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

                    Console.WriteLine();
                }
            }

            Profiler.TraceProfilerStatistics();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
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
                Console.WriteLine(proc.StandardOutput.ReadToEnd());
                Console.WriteLine(proc.StandardError.ReadToEnd());
                Thread.Sleep(10);
            }

            Console.WriteLine(proc.StandardOutput.ReadToEnd());
            Console.WriteLine(proc.StandardError.ReadToEnd());
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
        }
    }
}
