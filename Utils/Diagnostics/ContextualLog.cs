namespace CarbonCore.Utils.Diagnostics
{
    using System;

    using CarbonCore.Utils.Contracts;

    public sealed class ContextualLog : ILog
    {
        private readonly string context;
        private readonly ILog parent;

        public ContextualLog(string context, ILog parent)
        {
            this.context = context;
            this.parent = parent;
        }

        public void Warning(string message, params object[] args)
        {
            this.parent.Warning(string.Concat(this.context, "\t", message), args);
        }

        public void Error(string message, [System.Runtime.InteropServices.OptionalAttribute][System.Runtime.InteropServices.DefaultParameterValueAttribute(null)]Exception exception, params object[] args)
        {
            this.parent.Error(string.Concat(this.context, "\t", message), exception: exception, args: args);
        }

        public void Info(string message, params object[] args)
        {
            this.parent.Info(string.Concat(this.context, "\t", message), args);
        }

        public void Debug(string message, params object[] args)
        {
            this.parent.Debug(string.Concat(this.context, "\t", message), args);
        }
    }
}
