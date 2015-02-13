namespace CarbonCore.JSharpBridge.Core
{
    using System;

    public class LogRecord
    {
        public long GetMillis()
        {
            throw new System.NotImplementedException();
        }

        public LogLevel GetLevel()
        {
            throw new System.NotImplementedException();
        }

        public Exception GetThrown()
        {
            return Utils.Diagnostics.Internal.NotImplemented<Exception>();
        }
    }
}
