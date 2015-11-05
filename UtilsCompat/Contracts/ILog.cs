using System;

namespace CarbonCore.Utils.Compat.Contracts
{
    public interface ILog
    {
        string SourceName { get; }

        bool IsMuted { get; set; }

        void LogException(Exception exception);

        void Assert(bool condition, string message);

        void Warning(string message, params object[] args);

        void Error(string message, params object[] args);

        void Info(string message, params object[] args);

        void Debug(string message, params object[] args);
    }
}
