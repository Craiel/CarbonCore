namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.Utils.Compat.Contracts;

    public abstract class LogBase : ILog
    {
        private readonly TraceSource traceSource;

        private readonly IDictionary<int, ILogEvent> registeredEvents;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected LogBase(string sourceName)
        {
            this.traceSource = new TraceSource(sourceName);

            this.registeredEvents = new Dictionary<int, ILogEvent>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Debug(string message, params object[] args)
        {
            string formattedMessage = message;
            if (args != null && args.Length > 0)
            {
                formattedMessage = string.Format(message, args);
            }

            Trace.WriteLine(formattedMessage);
        }

        public void Warning(string message, params object[] args)
        {
            this.traceSource.TraceEvent(TraceEventType.Warning, -1, message, args);
        }

        public void Error(string message, Exception exception = null, params object[] args)
        {
            this.traceSource.TraceEvent(TraceEventType.Error, -1, message, args);
        }

        public void Info(string message, params object[] args)
        {
            this.traceSource.TraceInformation(message, args);
        }

        public void RegisterEvent(int id, ILogEvent data)
        {
            if (this.registeredEvents.ContainsKey(id))
            {
                throw new InvalidOperationException("Event already defined: " + id);
            }

            this.registeredEvents.Add(id, data);
        }

        public ILog AquireContextLog(string context)
        {
            return new ContextualLog(context, this);
        }
    }
}
