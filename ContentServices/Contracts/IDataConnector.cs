namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;

    using CarbonCore.Utils.Contracts;

    public interface IDataConnector : IDisposable
    {
        string NotNullStatement { get; }

        bool Connect();

        DbCommand CreateCommand(ISqlStatement statement = null);
        DbCommand CreateCommand(IList<ISqlStatement> statements);

        DbTransaction BeginTransaction();

        void Disconnect();
    }
}