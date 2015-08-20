namespace CarbonCore.ContentServices.Compat.Logic
{
    using System;
    using System.Data;

    public class SqlConnectorTransaction : IDisposable
    {
        private readonly IDbTransaction transaction;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SqlConnectorTransaction(IDbTransaction transaction)
        {
            this.transaction = transaction;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsDiposed { get; private set; }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IDbTransaction GetTransaction()
        {
            return this.transaction;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.IsDiposed = true;
                this.transaction.Dispose();
            }
        }
    }
}
