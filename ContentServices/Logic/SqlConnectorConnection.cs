namespace CarbonCore.ContentServices.Compat.Logic
{
    using System;
    using System.Data;
    using System.Threading;

    public class SqlConnectorConnection : IDisposable
    {
        private static int nextConnectionId = 1;

        private readonly IDbConnection connection;

        private SqlConnectorTransaction activeTransaction;
        private SqlConnectorCommand activeCommand;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlConnectorConnection(IDbConnection connection)
        {
            this.Id = nextConnectionId++;
            this.connection = connection;
            this.ThreadId = Thread.CurrentThread.ManagedThreadId;
            this.UseCount = 1;
            this.Created = DateTime.Now;
            this.LastUsed = this.Created;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Id { get; private set; }

        public int ThreadId { get; private set; }

        public int UseCount { get; set; }

        public DateTime Created { get; private set; }

        public DateTime LastUsed { get; set; }

        public bool HasTransaction
        {
            get
            {
                return this.activeTransaction != null && !this.activeTransaction.IsDiposed;
            }
        }

        public bool HasCommand
        {
            get
            {
                return this.activeCommand != null && !this.activeCommand.IsDiposed;
            }
        }

        public bool CanDispose
        {
            get
            {
                return !this.HasTransaction && !this.HasCommand;
            }
        }

        public bool IsDisposed { get; private set; }

        public ConnectionState State
        {
            get
            {
                return this.connection.State;
            }
        }

        public override bool Equals(object obj)
        {
            var other = obj as SqlConnectorConnection;
            if (other == null)
            {
                return false;
            }

            return other.ThreadId == this.ThreadId;
        }

        public override int GetHashCode()
        {
            return this.ThreadId.GetHashCode();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Open(string connectionString, bool waitForConnect = true)
        {
            this.connection.ConnectionString = connectionString;
            this.connection.Open();

            if (waitForConnect)
            {
                while (this.connection.State == ConnectionState.Connecting)
                {
                    Thread.Sleep(10);
                }
            }
        }

        public SqlConnectorTransaction BeginTransaction()
        {
            if (this.activeTransaction != null && !this.activeTransaction.IsDiposed)
            {
                throw new InvalidOperationException("Transaction is already active!");
            }

            this.activeTransaction = new SqlConnectorTransaction(this.connection.BeginTransaction());
            return this.activeTransaction;
        }

        public SqlConnectorCommand CreateCommand()
        {
            if (this.activeCommand != null && !this.activeCommand.IsDiposed)
            {
                throw new InvalidOperationException("Command is already active");
            }

            if (this.activeTransaction == null || this.activeTransaction.IsDiposed)
            {
                System.Diagnostics.Trace.TraceWarning("Creating command without active transaction!");
            }

            this.activeCommand = new SqlConnectorCommand(this.connection.CreateCommand());
            return this.activeCommand;
        }

        public void UpdateUse()
        {
            this.UseCount++;
            this.LastUsed = DateTime.Now;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.connection.Close();
                this.connection.Dispose();
                this.IsDisposed = true;
            }
        }
    }
}
