namespace CarbonCore.Utils.Compat.Contracts
{
    using System;

    public interface ILog
    {
        void Warning(string message, params object[] args);
        void Error(string message, Exception exception = null, params object[] args);
        void Info(string message, params object[] args);

        void Debug(string message, params object[] args);
    }

    public interface ILogBase
    {
        ILog AquireContextLog(string context);
    }
}
