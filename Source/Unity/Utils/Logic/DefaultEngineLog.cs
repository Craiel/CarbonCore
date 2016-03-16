namespace CarbonCore.Unity.Utils.Logic
{
    using System;

    using CarbonCore.Utils.Contracts;

    public class DefaultEngineLog : ILog
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DefaultEngineLog(string sourceName)
        {
            this.SourceName = sourceName;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string SourceName { get; private set; }

        public bool IsMuted { get; set; }

        public void Assert(bool condition, string message, params object[] args)
        {
            if (message == null)
            {
                UnityEngine.Assertions.Assert.IsTrue(condition);
                return;
            }

            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }

            UnityEngine.Assertions.Assert.IsTrue(condition, message);
        }

        public void Warning(string message, params object[] args)
        {
            if (this.IsMuted)
            {
                return;
            }

            if (args != null && args.Length > 0)
            {
                UnityEngine.Debug.LogWarningFormat(message, args);
                return;
            }

            UnityEngine.Debug.LogWarning(message);
        }

        public void Error(string message, params object[] args)
        {
            if (this.IsMuted)
            {
                return;
            }

            if (args != null && args.Length > 0)
            {
                UnityEngine.Debug.LogErrorFormat(message, args);
                return;
            }

            UnityEngine.Debug.LogError(message);
        }

        public void Info(string message, params object[] args)
        {
            if (this.IsMuted)
            {
                return;
            }

            if (args != null && args.Length > 0)
            {
                UnityEngine.Debug.LogFormat(message, args);
                return;
            }

            UnityEngine.Debug.Log(message);
        }

        public void Debug(string message, params object[] args)
        {
            this.Info(message, args);
        }

        public void LogException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
    }
}
