namespace CarbonCore.Processing.Resource
{
    using System;
    using System.IO;

    using CarbonCore.Processing.Contracts;

    public abstract class ProtocolResource : ICarbonResource
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public abstract void Load(Stream source);

        public abstract long Save(Stream target);

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
