namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Data.Common;

    public interface IDataConnector : IDisposable
    {
        string NotNullStatement { get; }

        bool Connect();

        DbCommand CreateCommand();

        void Disconnect();
    }
}
