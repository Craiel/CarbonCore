namespace CarbonCore.Utils.Diagnostics.TraceListeners
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;

    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Formatting;
    using CarbonCore.Utils.IO;

    /// <summary>
    /// Taken in large parts from Essential Diagnostics Project
    /// </summary>
    public class FileTraceListener : TraceListener
    {
        private const string DefaultFileNameTemplate = "{AssemblyName}-{DateTime:yyyy-MM-dd}.log";
        private const string DefaultTemplate = "{DateTime:u}\t{Source}({ThreadId})\t{EventType}\t{Id}\t{Message}";

        private readonly IFormatter formatter;
        private readonly ITextFile textFile;

        private readonly IList<string> traceCache;

        private string template = DefaultTemplate;

        private int maxRotation = 10;

        private bool attributesProcessed;
        private bool isRotating;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FileTraceListener()
            : this(DefaultFileNameTemplate)
        {
        }

        public FileTraceListener(string fileTemplate)
        {
            this.formatter = new Formatter();
            this.traceCache = new List<string>();
            this.textFile = new TextFile { File = new CarbonFile(this.formatter.Format(fileTemplate)) };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            if (!this.attributesProcessed)
            {
                this.ProcessAttributes();
            }
            
            this.formatter.Set("Source", source);
            this.formatter.Set("EventType", eventType.ToString());
            this.formatter.Set("Id", id.ToString(CultureInfo.InvariantCulture));
            this.formatter.Set("Message", args.Length > 0 ? string.Format(format, args) : format);

            lock (this.traceCache)
            {
                this.traceCache.Add(this.formatter.Format(this.template));
            }

            this.Flush();
        }

        public override void Write(string message)
        {
            if (!this.attributesProcessed)
            {
                this.ProcessAttributes();
            }

            this.WriteLine(message);
        }

        public override void WriteLine(string message)
        {
            if (!this.attributesProcessed)
            {
                this.ProcessAttributes();
            }

            this.SetFormatDefaults();
            this.formatter.Set("Message", message);
            lock (this.traceCache)
            {
                this.traceCache.Add(this.formatter.Format(this.template));
            }

            this.Flush();
        }

        public override void Flush()
        {
            if (this.isRotating)
            {
                return;
            }

            lock (this.traceCache)
            {
                foreach (string line in this.traceCache)
                {
                    this.textFile.WriteLine(line);
                }

                this.traceCache.Clear();
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override string[] GetSupportedAttributes()
        {
            return new[] { "template", "Template", "rotateFiles", "RotateFiles", "maxRotation", "MaxRotation" };
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ProcessAttributes()
        {
            lock (this.Attributes)
            {
                this.attributesProcessed = true;

                if (this.Attributes.ContainsKey("template"))
                {
                    this.template = this.Attributes["template"];
                    this.template = this.template.Replace("\\t", "\t"); // Gets escaped so we have to reverse this, can't see a better way atm
                }

                if (this.Attributes.ContainsKey("rotateFiles"))
                {
                    bool rotateFiles = bool.Parse(this.Attributes["rotateFiles"]);
                    if (rotateFiles)
                    {
                        this.RotateFiles();
                    }
                }

                if (this.Attributes.ContainsKey("maxRotation"))
                {
                    this.maxRotation = int.Parse(this.Attributes["maxRotation"]);
                }
            }
        }

        private void SetFormatDefaults()
        {
            this.formatter.Set("Source", "N/A");
            this.formatter.Set("EventType", "N/A");
            this.formatter.Set("Id", "N/A");
            this.formatter.Set("Message", "N/A");
        }

        private void RotateFiles()
        {
            this.isRotating = true;

            lock (this.textFile)
            {
                this.textFile.Close();
                this.textFile.File.Rotate(this.maxRotation);
            }

            this.isRotating = false;
        }
    }
}
