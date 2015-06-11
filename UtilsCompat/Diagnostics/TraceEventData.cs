namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System;
    using System.Diagnostics;

    public class TraceEventData
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        public string Source { get; set; }

        public TraceEventType Type { get; set; }

        public string Format { get; set; }

        public object[] Args { get; set; }
    }
}
