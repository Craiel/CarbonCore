﻿namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System.Diagnostics;

    using CarbonCore.Utils.Compat.Contracts;

    public class TraceLog : ILog
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TraceLog(string sourceName)
        {
            this.SourceName = sourceName;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string SourceName { get; private set; }

        public bool IsMuted { get; set; }

        public virtual void Debug(string message, params object[] args)
        {
            if (this.IsMuted)
            {
                return;
            }

            string formattedMessage = message;
            if (args != null && args.Length > 0)
            {
                formattedMessage = string.Format(message, args);
            }

            Trace.WriteLine(formattedMessage);
        }

        public void Assert(bool condition, string message = null)
        {
            Trace.Assert(condition, message ?? string.Empty);
        }

        public virtual void Warning(string message, params object[] args)
        {
            if (this.IsMuted)
            {
                return;
            }

            Trace.TraceWarning(this.PreformatMessage(message), args);
        }

        public virtual void Error(string message, params object[] args)
        {
            if (this.IsMuted)
            {
                return;
            }

            Trace.TraceError(this.PreformatMessage(message), args);
        }

        public virtual void Info(string message, params object[] args)
        {
            if (this.IsMuted)
            {
                return;
            }

            Trace.TraceInformation(this.PreformatMessage(message), args);
        }

        private string PreformatMessage(string message)
        {
            return string.Format("[{0}]\t{1}", this.SourceName, message);
        }


        public void LogException(System.Exception exception)
        {
            Trace.TraceError(this.PreformatMessage(exception.ToString()));
        }
    }
}
