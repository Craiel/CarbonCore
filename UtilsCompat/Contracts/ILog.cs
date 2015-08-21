namespace CarbonCore.Utils.Compat.Contracts
{
    using System;

    public interface ILog
    {
        string SourceName { get; }

        bool IsMuted { get; set; }

        void Warning(string message, params object[] args);

        void Error(string message, Exception exception = null, params object[] args);

        void Info(string message, params object[] args);

        void Debug(string message, params object[] args);
    }
}
