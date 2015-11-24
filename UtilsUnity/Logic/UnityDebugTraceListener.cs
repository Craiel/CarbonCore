namespace CarbonCore.Utils.Unity.Logic
{
    using System;
    using System.Diagnostics;
    using System.Linq;

    public class UnityDebugTraceListener : TraceListener
    {
        private const string MessageSegmentSeparator = " : ";

        private const string MessageSourceExtension = ".dll";

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Setup()
        {
            // Add a trace listener if there is non present yet
            if (Trace.Listeners.Cast<TraceListener>().Any(x => x is UnityDebugTraceListener))
            {
                return;
            }

            Trace.Listeners.Add(new UnityDebugTraceListener());
        }

        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string format, params object[] args)
        {
            // We don't get events in unity since TRACE is not defined by default
            //  have to deal with WriteLine instead since we can't enable it for in-editor scripts
            this.Flush();
        }

        public override void Write(string message)
        {
        }

        public override void WriteLine(string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            string[] segments = message.Split(new[] { MessageSegmentSeparator }, StringSplitOptions.RemoveEmptyEntries);
            string[] infoSegments = segments[0].Split(new[] { MessageSourceExtension }, StringSplitOptions.RemoveEmptyEntries);

            if (infoSegments.Length == 2 && segments.Length > 2)
            {
                string source = System.IO.Path.GetFileNameWithoutExtension(infoSegments[0]);
                TraceEventType type = (TraceEventType)Enum.Parse(typeof(TraceEventType), infoSegments[1]);
                int thread;
                int messageJoinIndex = 2;
                if (!int.TryParse(segments[1], out thread))
                {
                    thread = -1;
                    messageJoinIndex = 1;
                }

                string actualMessage = string.Join(MessageSegmentSeparator, segments, messageJoinIndex, segments.Length - messageJoinIndex);
                this.LogMessageToUnity(source, type, thread, actualMessage);
            }
            else
            {
                UnityEngine.Debug.LogWarning("<TRACE> " + message);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void LogMessageToUnity(string source, TraceEventType type, int thread, string message)
        {
            string formattedMessage = string.Format("[{0}]({1}) - {2}", source, thread, message);
            switch (type)
            {
                case TraceEventType.Error:
                case TraceEventType.Critical:
                    {
                        UnityEngine.Debug.LogError(formattedMessage);
                        break;
                    }

                case TraceEventType.Warning:
                    {
                        UnityEngine.Debug.LogWarning(formattedMessage);
                        break;
                    }

                default:
                    {
                        UnityEngine.Debug.Log(formattedMessage);
                        break;
                    }
            }
        }
    }
}
