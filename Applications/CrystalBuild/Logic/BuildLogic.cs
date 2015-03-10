namespace CarbonCore.Applications.CrystalBuild.Logic
{
    using System.Collections.Generic;
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
        }

        public void BuildTemplates(IList<CarbonFileResult> sources, CarbonFile target)
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} Templates into {1}", sources.Count, target);

            var processor = this.factory.Resolve<ITemplateProcessor>();
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
        }

        public void BuildData(IList<CarbonFileResult> sources, CarbonFile target)
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} Data into {1}", sources.Count, target);

            var processor = this.factory.Resolve<IExcelProcessor>();
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

            System.Diagnostics.Trace.TraceError("Not implemented!");
        }

        public void BuildStyleSheets(IList<CarbonFileResult> sources, CarbonFile target)
        {
            System.Diagnostics.Trace.TraceInformation("Building {0} Stylesheets into {1}", sources.Count, target);

            var processor = this.factory.Resolve<ICssProcessor>();
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
        }

        public void CopyContents(IList<CarbonFileResult> sources, CarbonDirectory target)
        {
            System.Diagnostics.Trace.TraceInformation("Copying {0} Content into {1}", sources.Count, target);

            foreach (CarbonFileResult source in sources)
            {
                source.Absolute.CopyTo(target.ToFile(source.Relative), true);
            }
        }
    }
}
