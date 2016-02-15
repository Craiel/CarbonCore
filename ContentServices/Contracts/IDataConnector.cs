namespace CarbonCore.ContentServices.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    
    using CarbonCore.Utils.Contracts;

    public interface IDataConnector : IDisposable
    {
        string NotNullStatement { get; }

        bool Connect();

        IDbCommand CreateCommand(ISqlStatement statement = null);
        IDbCommand CreateCommand(IList<ISqlStatement> statements);

        IDbTransaction BeginTransaction();

        void Disconnect();
    }
}