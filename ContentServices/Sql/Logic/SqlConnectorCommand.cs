namespace CarbonCore.ContentServices.Sql.Logic
{
    using System;
    using System.Data;

    public class SqlConnectorCommand : IDisposable
    {
        private readonly IDbCommand command;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlConnectorCommand(IDbCommand command)
        {
            this.command = command;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsDiposed { get; private set; }

        public IDbCommand GetCommand()
        {
            return this.command;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.IsDiposed = true;
                this.command.Dispose();
            }
        }
    }
}