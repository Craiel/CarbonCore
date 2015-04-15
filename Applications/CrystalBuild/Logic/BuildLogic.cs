namespace CarbonCore.Applications.CrystalBuild.Logic
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.IO;
    using System.Text;

    using CarbonCore.Utils.Contracts.IoC;
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
        public void Build(IList<CarbonFileResult> sources, CarbonFile target, bool isDebug = false)
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} Sources into {1}", sources.Count, target);

            var processor = this.factory.Resolve<IJavaScriptProcessor>();
            processor.IsDebug = isDebug;
            processor.SetContext(new ProcessingContext());
            foreach (CarbonFileResult source in sources)
            {
                processor.Process(source.Absolute);
            }

            target.GetDirectory().Create();
            using (FileStream stream = target.OpenCreate())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    writer.Write(processor.GetData());
                }
            }

            this.TraceProcessorResult(processor, "Building Sources");
        }

        public void BuildTemplates(IList<CarbonFileResult> sources, CarbonFile target)
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} Templates into {1}", sources.Count, target);

            var processor = this.factory.Resolve<ITemplateProcessor>();
            processor.SetContext(new ProcessingContext());
            for (int i = 0; i < sources.Count; i++)
            {
                processor.Process(sources[i].Absolute);
            }
            
            target.GetDirectory().Create();
            using (FileStream stream = target.OpenCreate())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    writer.Write(processor.GetData());
                }
            }

            this.TraceProcessorResult(processor, "Building Templates");
        }

        public void BuildData(IList<CarbonFileResult> sources, CarbonFile target)
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} Data into {1}", sources.Count, target);

            var processor = this.factory.Resolve<IExcelProcessor>();
            processor.SetContext(new ProcessingContext());
            foreach (CarbonFileResult file in sources)
            {
                System.Diagnostics.Trace.TraceInformation("  {0}", file.Absolute.FileName);
                processor.Process(file.Absolute);
            }

            using (var stream = target.OpenCreate())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    writer.Write(processor.GetData());
                }
            }

            this.TraceProcessorResult(processor, "Building Data");
        }

        public void BuildStyleSheets(IList<CarbonFileResult> sources, CarbonFile target)
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} Stylesheets into {1}", sources.Count, target);

            var processor = this.factory.Resolve<ICssProcessor>();
            processor.SetContext(new ProcessingContext());
            foreach (CarbonFileResult file in sources)
            {
                processor.Process(file.Absolute);
            }

            target.GetDirectory().Create();
            using (FileStream stream = target.OpenCreate())
            {
                using (var writer = new StreamWriter(stream, Encoding.UTF8, 4096, true))
                {
                    writer.Write(processor.GetData());
                }
            }

            this.TraceProcessorResult(processor, "Building Stylesheets");
        }

        public void CopyContents(IList<CarbonFileResult> sources, CarbonDirectory target)
        {
            System.Diagnostics.Trace.TraceInformation("Copying {0} Content into {1}", sources.Count, target);

            foreach (CarbonFileResult source in sources)
            {
                source.Absolute.CopyTo(target.ToFile(source.Relative), true);
            }
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
