namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System.Text;

    using CarbonCore.Utils;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts.Processors;

    public abstract class ContentProcessor : IContentProcessor
    {
        private readonly StringBuilder builder;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ContentProcessor()
        {
            this.builder = new StringBuilder();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ProcessingContext Context { get; private set; }

        public void Process(CarbonFile source)
        {
            Diagnostic.Assert(this.Context != null, "Context must be set!");
            this.DoProcess(source);
        }

        public string GetData()
        {
            Diagnostic.Assert(this.Context != null, "Context must be set!");

            this.PreprocessData();
            return this.PostProcessData(this.builder.ToString());
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected abstract void DoProcess(CarbonFile source);

        protected void Append(string content = "")
        {
            this.builder.Append(content);
        }

        protected void AppendLine(string line = "")
        {
            this.builder.AppendLine(line);
        }

        protected void AppendFormat(string format, params object[] args)
        {
            this.builder.AppendFormat(format, args);
        }

        protected void AppendFormatLine(string format, params object[] args)
        {
            this.builder.AppendFormatLine(format, args);
        }

        protected virtual void PreprocessData()
        {
        }

        protected virtual string PostProcessData(string data)
        {
            return data;
        }

        public void SetContext(ProcessingContext context)
        {
            this.Context = context;
        }
    }
}
