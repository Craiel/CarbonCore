namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts;
    using CrystalBuild.Contracts.Processors;

    public class TemplateProcessor : ContentProcessor, ITemplateProcessor
    {
        private const string DataPrefix = @"declare(""TemplateContent"", function() { return {";
        private const string DataSuffix = "}; });";
        
        private static readonly char[] StripFromTemplates = { '\n', '\r', '\t' };

        private readonly IBuildUtils buildUtils;

        private readonly IList<string> templateSegments;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TemplateProcessor(IFactory factory)
        {
            this.buildUtils = factory.Resolve<IBuildUtils>();

            this.templateSegments = new List<string>();

            this.AppendLine(DataPrefix);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Process(CarbonFile file)
        {
            string content = file.ReadAsString();
            string[] segments = content.Split(StripFromTemplates, StringSplitOptions.RemoveEmptyEntries);
            content = string.Join(" ", segments);
            this.templateSegments.Add(string.Format("{0}: '{1}'", file.FileNameWithoutExtension, content));
        }

        protected override void PreprocessData()
        {
            for (int i = 0; i < this.templateSegments.Count; i++)
            {
                this.AppendFormat("\t{0}", this.templateSegments[i]);
                if (i < this.templateSegments.Count - 1)
                {
                    this.Append(",");
                }

                this.AppendLine();
            }
        }

        protected override string PostProcessData(string data)
        {
            return string.Concat(data, Environment.NewLine, DataSuffix);
        }
    }
}
