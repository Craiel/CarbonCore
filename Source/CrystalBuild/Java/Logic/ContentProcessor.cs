namespace CarbonCore.CrystalBuild.Java.Logic
{
    using System.Diagnostics;
    using System.Text;

    using CarbonCore.CrystalBuild.Contracts;
    using CarbonCore.Utils;
    using CarbonCore.Utils.IO;

    public abstract class ContentProcessor : IContentProcessor
    {
        private readonly StringBuilder builder;

        private IProcessingContext context;

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
        public void Process(CarbonFile source)
        {
            Debug.Assert(this.context != null, "Context must be set!");
            this.DoProcess(source);
        }

        public string GetData()
        {
            Debug.Assert(this.context != null, "Context must be set!");

            this.PreprocessData();
            return this.PostProcessData(this.builder.ToString());
        }

        public void SetContext<T>(T newContext)
            where T : class, IProcessingContext
        {
            this.context = newContext;
        }

        public T GetContext<T>()
            where T : class, IProcessingContext
        {
            return this.context as T;
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
    }
}
