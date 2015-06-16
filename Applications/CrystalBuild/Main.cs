namespace CarbonCore.Applications.CrystalBuild
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.Applications.CrystalBuild.Logic;
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.I18N;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.I18N;
    using CarbonCore.UtilsCommandLine.Contracts;

    using CrystalBuild.Contracts;
    
    public class Main : IMain
    {
        // --compilation_level ADVANCED_OPTIMIZATIONS
        public const string ClosureCompilerCommand = @"-jar ""{0}compiler.jar""  --js ""{1}"" --js_output_file {2} --language_in=ECMASCRIPT5 --externs ""{3}""";

        private readonly ICommandLineArguments arguments;
        private readonly IConfig config;
        private readonly IBuildLogic logic;

        private CarbonFile configFileName;

        private bool useDebug;
        private bool useClosure;

        private string targetLanguage;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Main(IFactory factory)
        {
            this.config = factory.Resolve<IConfig>();
            this.logic = factory.Resolve<IBuildLogic>();

            this.arguments = factory.Resolve<ICommandLineArguments>();
            this.RegisterCommandLineArguments();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Build()
        {
            // Set the default locale to english
            Localization.CurrentCulture = LocaleConstants.LocaleEnglishUS;

            if (!this.arguments.ParseCommandLineArguments() || this.configFileName == null)
            {
                this.arguments.PrintArgumentUse();
                return;
            }

            if (!string.IsNullOrEmpty(this.targetLanguage))
            {
                Localization.CurrentCulture = LocaleConstants.GetCulture(this.targetLanguage);
            }

            this.config.Load(this.configFileName);
            this.DoBuildProject();

            Localization.SaveDictionaries();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoBuildProject()
        {
            var cache = new ProcessingCache();

            if (this.config.Current.Images != null)
            {
                IList<CarbonDirectoryFilter> filters = this.config.Current.Images;
                IList<CarbonFileResult> files = CarbonDirectory.GetFiles(filters);
                if (files.Count > 0)
                {
                    var context = new ProcessingContext(cache) { Root = this.config.Current.ImageRoot };

                    this.logic.BuildImages(files, context);
                }
                else
                {
                    Trace.TraceWarning("No resources to register!");
                }
            }

            if (this.config.Current.Data != null)
            {
                IList<CarbonDirectoryFilter> filters = this.config.Current.Data;
                IList<CarbonFileResult> files = CarbonDirectory.GetFiles(filters);
                if (files != null && files.Count > 0)
                {
                    this.logic.BuildData(files, this.config.Current.ProjectRoot.ToFile(this.config.Current.DataTarget), new ProcessingContext(cache));
                }
                else
                {
                    Trace.TraceWarning("No data found to build!");
                }
            }

            if (this.config.Current.Templates != null)
            {
                IList<CarbonDirectoryFilter> filters = this.config.Current.Templates;
                IList<CarbonFileResult> files = CarbonDirectory.GetFiles(filters);
                if (files != null && files.Count > 0)
                {
                    this.logic.BuildTemplates(files, this.config.Current.ProjectRoot.ToFile(this.config.Current.TemplateTarget), new ProcessingContext(cache));
                }
                else
                {
                    Trace.TraceWarning("No templates found to build!");
                }
            }

            if (this.config.Current.Sources != null)
            {
                IList<CarbonDirectoryFilter> filters = this.config.Current.Sources;
                IList<CarbonFileResult> files = CarbonDirectory.GetFiles(filters);
                CarbonFileResult mainFile = null;

                // Check if we have a specific main file
                if (this.config.Current.SourceMain != null)
                {
                    // Now we try to find the entry point file
                    foreach (var file in files)
                    {
                        if (file.Absolute.GetPath()
                            .EndsWith(this.config.Current.SourceMain.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            mainFile = file;
                            break;
                        }
                    }

                    if (mainFile == null)
                    {
                        Trace.TraceError(
                            "Could not find entry point: {0} ({1} files)",
                            this.config.Current.SourceMain,
                            files.Count);
                        return;
                    }

                    // re-add the main file so it ends up as last in the block
                    files.Remove(mainFile);
                    files.Add(mainFile);
                }

                if (files.Count > 0)
                {
                    CarbonFile targetFile = this.config.Current.ProjectRoot.ToFile(this.config.Current.SourceTarget);
                    CarbonFile targetFileClosure = targetFile;
                    if (this.useClosure)
                    {
                        targetFile = targetFile.GetDirectory().ToFile(targetFile.FileNameWithoutExtension + "_raw.js");
                    }

                    this.logic.Build(
                        files,
                        targetFile,
                        new ProcessingContext(cache)
                        {
                            Name = this.config.Current.Name,
                            IsDebug = this.useDebug,
                            ExportSourceAsModule = this.config.Current.ExportSourceAsModule
                        });

                    if (this.useClosure)
                    {
                        var info = new ProcessStartInfo("java.exe", string.Format(ClosureCompilerCommand, RuntimeInfo.Assembly.GetDirectory().ToRelative<CarbonDirectory>(RuntimeInfo.WorkingDirectory), targetFile, targetFileClosure, RuntimeInfo.WorkingDirectory.ToFile("externs.js")))
                                       {
                                           UseShellExecute = false,
                                           WorkingDirectory = RuntimeInfo.WorkingDirectory.ToString()
                                       };
                        var proc = Process.Start(info);
                        proc.WaitForExit();
                    }
                }
                else
                {
                    Trace.TraceWarning("No source files found to build!");
                }
            }

            if (this.config.Current.StyleSheets != null)
            {
                IList<CarbonDirectoryFilter> filters = this.config.Current.StyleSheets;
                IList<CarbonFileResult> files = CarbonDirectory.GetFiles(filters);
                if (files != null && files.Count > 0)
                {
                    this.logic.BuildStyleSheets(files, this.config.Current.ProjectRoot.ToFile(this.config.Current.StyleSheetTarget), new ProcessingContext(cache));
                }
                else
                {
                    Trace.TraceWarning("No templates found to build!");
                }
            }

            if (this.config.Current.Contents != null)
            {
                IList<CarbonDirectoryFilter> filters = this.config.Current.Contents;
                IList<CarbonFileResult> files = CarbonDirectory.GetFiles(filters);
                if (files.Count > 0)
                {
                    this.logic.CopyContents(files, this.config.Current.ProjectRoot.ToDirectory(this.config.Current.ContentTarget));
                }
                else
                {
                    Trace.TraceWarning("No content files found to copy!");
                }
            }

            foreach (string key in cache.ImageUseCount.Keys)
            {
                if (cache.ImageUseCount[key] <= 0)
                {
                    Trace.TraceWarning("Unused image: {0}", key);
                }
            }
        }
        
        private void RegisterCommandLineArguments()
        {
            ICommandLineSwitchDefinition definition = this.arguments.Define("p", "projectFile", x => this.configFileName = new CarbonFile(x));
            definition.RequireArgument = true;
            definition.Description = "The project file to compile";

            definition = this.arguments.Define("d", "debug", x => this.useDebug = true);
            definition.Description = "Build with debug info enabled";

            definition = this.arguments.Define("c", "closure", x => this.useClosure = true);
            definition.Description = "Run closure on the target script file";

            definition = this.arguments.Define("l", "language", x => this.targetLanguage = x);
            definition.RequireArgument = true;
            definition.Description = "Set the language to build (en, fr, de ...)";
        }
    }
}
