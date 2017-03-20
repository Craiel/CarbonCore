namespace CarbonCore.Unity.Utils.Logic.Logging
{
    using System;

    using NLog;
    using NLog.Targets;

    public class NLogInterceptTarget : TargetWithLayout
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action<LogEventInfo> OnEventReceived;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Write(LogEventInfo logEvent)
        {
            if (this.OnEventReceived != null)
            {
                this.OnEventReceived(logEvent);
            }
        }
    }
}
