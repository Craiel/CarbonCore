namespace CarbonCore.Utils.Edge.Diagnostics.TraceListeners
{
    using System.Diagnostics;
    
    public class Log4NetTraceListener : TraceListener
    {
        private const string DefaultLogger = "trace";

        private const string LoggerAttributeKey = "logger";
        private const string LoggerAttributeName = "Logger";

        private string logger = DefaultLogger;

        private bool attributesProcessed;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            this.ProcessAttributes();

            log4net.ILog log = log4net.LogManager.GetLogger(this.logger);
            
            switch (eventType)
            {
                case TraceEventType.Critical:
                    {
                        log.FatalFormat(format, args);
                        break;
                    }

                case TraceEventType.Error:
                    {
                        log.ErrorFormat(format, args);
                        break;
                    }

                case TraceEventType.Warning:
                    {
                        log.WarnFormat(format, args);
                        break;
                    }

                default:
                    {
                        log.InfoFormat(format, args);
                        break;
                    }
            }

            this.Flush();
        }

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override string[] GetSupportedAttributes()
        {
            return new[] { LoggerAttributeKey, LoggerAttributeName };
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void ProcessAttributes()
        {
            if (this.attributesProcessed)
            {
                return;
            }

            lock (this.Attributes)
            {
                this.attributesProcessed = true;

                if (this.Attributes.ContainsKey(LoggerAttributeKey))
                {
                    this.logger = this.Attributes[LoggerAttributeKey];
                }
            }
        }
    }
}
