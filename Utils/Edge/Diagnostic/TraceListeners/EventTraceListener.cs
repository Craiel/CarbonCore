namespace CarbonCore.Utils.Edge.Diagnostic.TraceListeners
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.Utils.Diagnostics;

    public class EventTraceListener : TraceListener
    {
        private static readonly ConcurrentQueue<TraceEventData> EventData;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static EventTraceListener()
        {
            EventData = new ConcurrentQueue<TraceEventData>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IList<TraceEventData> PollEventData()
        {
            int entryCount = EventData.Count;
            var result = new List<TraceEventData>(entryCount);
            for (var i = 0; i < entryCount; i++)
            {
                TraceEventData data;
                if (EventData.TryDequeue(out data))
                {
                    result.Add(data);   
                }
            }

            return result;
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            var data = new TraceEventData
                           {
                               Id = id,
                               Source = source,
                               Type = eventType,
                               Format = format,
                               Args = args,
                               Time = DateTime.Now
                           };

            EventData.Enqueue(data);

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
