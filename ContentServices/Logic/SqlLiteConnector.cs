namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Threading;

    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Database;

    public class SqlLiteConnector : ISqlLiteConnector
    {
        public const string SqlNotNull = " NOT NULL";
        public const string SqlLastId = "SELECT last_insert_rowid()";

        private const string InMemoryIdentifier = ":memory:";

        private readonly SQLiteFactory factory;

        private SQLiteConnection connection;

        private CarbonFile file;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlLiteConnector()
        {
            this.factory = new SQLiteFactory();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string NotNullStatement
        {
            get
            {
                return SqlNotNull;
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetFile(CarbonFile newFile)
        {
            System.Diagnostics.Trace.Assert(this.connection == null, "SetFile must be called while disconnected!");

            this.file = newFile;
        }

        public bool Connect()
        {
            System.Diagnostics.Trace.Assert(this.connection == null);

            this.connection = this.factory.CreateConnection() as SQLiteConnection;
            if (this.connection == null)
            {
                System.Diagnostics.Trace.TraceError("Could not create connection");
                return false;
            }
            
            string connectionTarget = InMemoryIdentifier;
            if (this.file != null)
            {
                connectionTarget = this.file.GetPath();
                if (!this.file.Exists)
                {
                    this.file.GetDirectory().Create();
                    SQLiteConnection.CreateFile(this.file.GetPath());
                }
            }
            
            this.connection.ConnectionString = string.Format("Data Source={0}", connectionTarget);
            return this.OpenConnection();
        }

        public DbCommand CreateCommand(ISqlStatement statement)
        {
            System.Diagnostics.Trace.Assert(this.connection != null);

            DbCommand command = this.connection.CreateCommand();
            if (statement != null)
            {
                System.Diagnostics.Trace.TraceInformation("SQLite: {0}", statement);
                statement.IntoCommand(command);
            }

            return command;
        }

        public DbCommand CreateCommand(IList<ISqlStatement> statements)
        {
            System.Diagnostics.Trace.Assert(this.connection != null && statements != null && statements.Count > 0);

            DbCommand command = this.connection.CreateCommand();
            command.CommandText = string.Format("{0};", Compat.Constants.StatementBegin);
            for (int i = 0; i < statements.Count; i++)
            {
                ISqlStatement statement = statements[i];
                statement.IntoCommand(command, string.Format("_{0}", i));
            }

            command.CommandText = string.Format("{0}\n{1};", command.CommandText, Compat.Constants.StatementCommit);

            return command;
        }

        public DbTransaction BeginTransaction()
        {
            return this.connection.BeginTransaction();
        }

        public void Disconnect()
        {
            if (this.connection != null)
            {
                this.connection.Dispose();
                this.connection = null;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.Disconnect();
                this.factory.Dispose();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool OpenConnection()
        {
            this.connection.Open();
            while (this.connection.State == ConnectionState.Connecting)
            {
                Thread.Sleep(100);
            }

            return this.connection.State == ConnectionState.Open;
        }
    }
}
