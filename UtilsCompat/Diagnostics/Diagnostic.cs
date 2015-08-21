namespace CarbonCore.Utils.Compat.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Threading;

    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.Threading;

    public class Diagnostic
    {
        private static readonly CarbonDiagnostics Instance = new CarbonDiagnostics();

        private static readonly IDictionary<int, EngineTime> ThreadTimes = new Dictionary<int, EngineTime>();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Debug(string message, params object[] args)
        {
            GetThreadContext().Debug(PreformatMessage(message), args);
        }

        public static void Warning(string message, params object[] args)
        {
            GetThreadContext().Warning(PreformatMessage(message), args);
        }

        public static void Error(string message)
        {
            GetThreadContext().Error(PreformatMessage(message));
        }

        public static void Error(string message, Exception exception, params object[] args)
        {
            GetThreadContext().Error(PreformatMessage(message), exception, args);
        }
        
        public static void Info(string message, params object[] args)
        {
            GetThreadContext().Info(PreformatMessage(message), args);
        }

        public static void Assert(bool condition, string message = null)
        {
            System.Diagnostics.Trace.Assert(condition, message ?? string.Empty);
        }

        public static void RegisterThread(string name, EngineTime time = null)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (time != null)
            {
                lock (ThreadTimes)
                {
                    ThreadTimes.Add(threadId, time);
                }
            }

            Instance.RegisterLogContext(threadId, string.Format("({0}) {1}", threadId, name));
        }

        public static void UnregisterThread()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            lock (ThreadTimes)
            {
                if (ThreadTimes.ContainsKey(threadId))
                {
                    ThreadTimes.Remove(threadId);
                }
            }

            Instance.UnregisterLogContext(threadId);
        }

        public static void SetMute(int managedThreadId, bool mute = true)
        {
            Instance.SetMute(managedThreadId, mute);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static ILog GetThreadContext()
        {
            return Instance.GetLogContext(Thread.CurrentThread.ManagedThreadId);
        }

        private static string PreformatMessage(string message)
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (ThreadTimes.ContainsKey(threadId))
            {
                return string.Format("{0} {1}", ThreadTimes[threadId].Time, message);
            }

            return message;
        }
    }
}
