namespace CarbonCore.ContentServices.Logic
{
    using System.Data;
    using System.Data.Common;
    using System.Data.SQLite;
    using System.Threading;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.Utils.IO;

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

        public virtual void Dispose()
        {
            this.Disconnect();
        }

        public void SetFile(CarbonFile newFile)
        {
            System.Diagnostics.Trace.Assert(this.connection == null, "SetFile must be called while disconnected!");

            this.file = newFile;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
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

        public DbCommand CreateCommand()
        {
            System.Diagnostics.Trace.Assert(this.connection != null);

            return this.connection.CreateCommand();
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
