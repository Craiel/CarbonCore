namespace CarbonCore.Applications.CrystalBuild.Logic
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts;
    using CrystalBuild.Contracts.Processors;
    
    public class BuildLogic : IBuildLogic
    {
        private readonly IFactory factory;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public BuildLogic(IFactory factory)
        {
            this.factory = factory;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Build(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context)
        {
            if (context.ExportSourceAsModule)
            {
                // Todo
            }

            Diagnostic.Info("Building {0} {1} into {2}", sources.Count, "Sources", target);

            var processor = this.factory.Resolve<IJavaScriptProcessor>();
            Diagnostic.Assert(processor != null);

            processor.SetContext(context);
            foreach (CarbonFileResult file in sources)
            {
                Diagnostic.Info("  {0}", file.Absolute.FileName);
                processor.Process(file.Absolute);
            }

            CarbonDirectory targetDirectory = target.GetDirectory();
            if (!targetDirectory.IsNull && !targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            using (var stream = target.OpenCreate())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    if (context.ExportSourceAsModule)
                    {
                        writer.Write("declare('{0}', function() {{\n", context.Name);
                    }

                    writer.Write(processor.GetData());

                    if (context.ExportSourceAsModule)
                    {
                        writer.Write("});");
                    }
                }
            }

            this.TraceProcessorResult(processor, "Building Sources");
        }

        public void BuildTemplates(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context)
        {
            this.DoBuildMultipleToOne<ITemplateProcessor>("Templates", sources, target, context);
        }

        public void BuildData(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context)
        {
            this.DoBuildMultipleToOne<IExcelProcessor>("Data", sources, target, context, useTempFileForProcessing: true);
        }

        public void BuildStyleSheets(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context)
        {
            this.DoBuildMultipleToOne<ICssProcessor>("Style-sheets", sources, target, context);
        }

        public void BuildImages(IList<CarbonFileResult> sources, ProcessingContext context)
        {
            this.DoBuildMultiple<IImageProcessor>("Images", sources, context);
        }

        public void CopyContents(IList<CarbonFileResult> sources, CarbonDirectory target)
        {
            Diagnostic.Info("Copying {0} Content into {1}", sources.Count, target);

            foreach (CarbonFileResult source in sources)
            {
                source.Absolute.CopyTo(target.ToFile(source.Relative), true);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoBuildMultipleToOne<T>(string buildName, IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context, bool useTempFileForProcessing = false)
            where T : IContentProcessor
        {
            Diagnostic.Info("Building {0} {1} into {2}", sources.Count, buildName, target);

            var processor = this.factory.Resolve(typeof(T)) as IContentProcessor;
            Diagnostic.Assert(processor != null);

            processor.SetContext(context);
            foreach (CarbonFileResult file in sources)
            {
                Diagnostic.Info("  {0}", file.Absolute.FileName);
                if (useTempFileForProcessing)
                {
                    CarbonFile tempFile = CarbonFile.GetTempFile();
                    tempFile.DeleteIfExists();

                    file.Absolute.CopyTo(tempFile);
                    processor.Process(tempFile);
                }
                else
                {
                    processor.Process(file.Absolute);
                }
            }

            CarbonDirectory targetDirectory = target.GetDirectory();
            if (!targetDirectory.IsNull && !targetDirectory.Exists)
            {
                targetDirectory.Create();
            }

            using (var stream = target.OpenCreate())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    writer.Write(processor.GetData());
                }
            }

            this.TraceProcessorResult(processor, $"Building {buildName}");
        }

        private void DoBuildMultiple<T>(string buildName, IList<CarbonFileResult> sources, ProcessingContext context)
            where T : IContentProcessor
        {
            Diagnostic.Info("Building {0} files for {1}", sources.Count, buildName);

            var processor = this.factory.Resolve(typeof(T)) as IContentProcessor;
            Diagnostic.Assert(processor != null);

            processor.SetContext(context);
            foreach (CarbonFileResult file in sources)
            {
                Diagnostic.Info("  {0}", file.Absolute.FileName);
                processor.Process(file.Absolute);
            }
            
            this.TraceProcessorResult(processor, $"Building {buildName}");
        }

        private void TraceProcessorResult(IContentProcessor processor, string name)
        {
            Diagnostic.Info("");
            Diagnostic.Info("Result for {0}", name);
            Diagnostic.Info(" -------------------");
            foreach (string warning in processor.Context.Warnings)
            {
                Diagnostic.Warning(" - WARNING: {0}", warning);
            }

            foreach (string error in processor.Context.Errors)
            {
                Diagnostic.Error(" - ERROR: {0}", error);
            }

            Diagnostic.Info("");
            Diagnostic.Info("{0} Done with {1} Errors, {2} Warnings\n\n", name, processor.Context.Errors.Count, processor.Context.Warnings.Count);
        }
    }
}
