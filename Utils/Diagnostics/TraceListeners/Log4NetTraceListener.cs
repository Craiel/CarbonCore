namespace CarbonCore.Utils.Diagnostics.TraceListeners
{
    using System.Diagnostics;

    public class Log4NetTraceListener : TraceListener
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            //

            this.Flush();
        }

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
        }
    }
}
