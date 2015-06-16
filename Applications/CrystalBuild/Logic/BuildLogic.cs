namespace CarbonCore.Applications.CrystalBuild.Logic
{
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.IO;

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

            System.Diagnostics.Trace.TraceInformation("Building {0} {1} into {2}", sources.Count, "Sources", target);

            var processor = this.factory.Resolve<IJavaScriptProcessor>();
            System.Diagnostics.Trace.Assert(processor != null);

            processor.SetContext(context);
            foreach (CarbonFileResult file in sources)
            {
                System.Diagnostics.Trace.TraceInformation("  {0}", file.Absolute.FileName);
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

            this.TraceProcessorResult(processor, string.Format("Building {0}", "Sources"));
        }

        public void BuildTemplates(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context)
        {
            this.DoBuildMultipleToOne<ITemplateProcessor>("Templates", sources, target, context);
        }

        public void BuildData(IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context)
        {
            this.DoBuildMultipleToOne<IExcelProcessor>("Data", sources, target, context);
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
            System.Diagnostics.Trace.TraceInformation("Copying {0} Content into {1}", sources.Count, target);

            foreach (CarbonFileResult source in sources)
            {
                source.Absolute.CopyTo(target.ToFile(source.Relative), true);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoBuildMultipleToOne<T>(string buildName, IList<CarbonFileResult> sources, CarbonFile target, ProcessingContext context)
            where T : IContentProcessor
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} {1} into {2}", sources.Count, buildName, target);

            var processor = this.factory.Resolve(typeof(T)) as IContentProcessor;
            System.Diagnostics.Trace.Assert(processor != null);

            processor.SetContext(context);
            foreach (CarbonFileResult file in sources)
            {
                System.Diagnostics.Trace.TraceInformation("  {0}", file.Absolute.FileName);
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
                    writer.Write(processor.GetData());
                }
            }

            this.TraceProcessorResult(processor, string.Format("Building {0}", buildName));
        }

        private void DoBuildMultiple<T>(string buildName, IList<CarbonFileResult> sources, ProcessingContext context)
            where T : IContentProcessor
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} files for {1}", sources.Count, buildName);

            var processor = this.factory.Resolve(typeof(T)) as IContentProcessor;
            System.Diagnostics.Trace.Assert(processor != null);

            processor.SetContext(context);
            foreach (CarbonFileResult file in sources)
            {
                System.Diagnostics.Trace.TraceInformation("  {0}", file.Absolute.FileName);
                processor.Process(file.Absolute);
            }
            
            this.TraceProcessorResult(processor, string.Format("Building {0}", buildName));
        }

        private void TraceProcessorResult(IContentProcessor processor, string name)
        {
            System.Diagnostics.Trace.TraceInformation("");
            System.Diagnostics.Trace.TraceInformation("Result for {0}", name);
            System.Diagnostics.Trace.TraceInformation(" -------------------");
            foreach (string warning in processor.Context.Warnings)
            {
                System.Diagnostics.Trace.TraceWarning(" - WARNING: {0}", warning);
            }

            foreach (string error in processor.Context.Errors)
            {
                System.Diagnostics.Trace.TraceError(" - ERROR: {0}", error);
            }

            System.Diagnostics.Trace.TraceInformation("");
            System.Diagnostics.Trace.TraceInformation("{0} Done with {1} Errors, {2} Warnings\n\n", name, processor.Context.Errors.Count, processor.Context.Warnings.Count);
        }
    }
}
