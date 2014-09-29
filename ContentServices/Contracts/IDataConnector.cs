namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Data.Common;

    using CarbonCore.Utils.Database;

    public interface IDataConnector : IDisposable
    {
        string NotNullStatement { get; }

        bool Connect();

        DbCommand CreateCommand(SqlStatement statement = null);

        DbTransaction BeginTransaction();

        void Disconnect();
    }
}