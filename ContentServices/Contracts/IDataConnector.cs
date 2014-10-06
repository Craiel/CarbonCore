namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    using CarbonCore.Utils.Database;

    public interface IDataConnector : IDisposable
    {
        string NotNullStatement { get; }

        bool Connect();

        DbCommand CreateCommand(SqlStatement statement = null);
        DbCommand CreateCommand(IList<SqlStatement> statements);

        DbTransaction BeginTransaction();

        void Disconnect();
    }
}