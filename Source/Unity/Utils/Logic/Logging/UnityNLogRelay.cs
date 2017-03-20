namespace CarbonCore.Unity.Utils.Logic.Logging
{
    using NLog;

    using UnityEngine;

    public class UnityNLogRelay : UnitySingleton<UnityNLogRelay>
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        static UnityNLogRelay()
        {
            Application.logMessageReceivedThreaded += OnUnityLogMessage;

            UnityEngine.Debug.Log("Unity NLog Relay Initialized");
        }

        private static void OnUnityLogMessage(string message, string stacktrace, LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                case LogType.Error:
                    {
                        if (string.IsNullOrEmpty(stacktrace))
                        {
                            Logger.Error(message);
                        }
                        else
                        {
                            Logger.Error(new UnityStackTraceException(stacktrace), message);
                        }

                        break;
                    }

                case LogType.Warning:
                    {
                        if (string.IsNullOrEmpty(stacktrace))
                        {
                            Logger.Warn(message);
                        }
                        else
                        {
                            Logger.Warn(new UnityStackTraceException(stacktrace), message);
                        }

                        break;
                    }

                case LogType.Log:
                    {
                        if (string.IsNullOrEmpty(stacktrace))
                        {
                            Logger.Info(message);
                        }
                        else
                        {
                            Logger.Info(new UnityStackTraceException(stacktrace), message);
                        }

                        break;
                    }
            }
        }
    }
}
