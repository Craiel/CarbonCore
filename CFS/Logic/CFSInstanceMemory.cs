namespace CarbonCore.CFS.Logic
{
    using System.IO;

    public class CFSInstanceMemory : CFSInstance
    {
        private readonly MemoryStream stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CFSInstanceMemory()
        {
            this.stream = new MemoryStream();
            this.Initialize(this.stream);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            if (isDisposing)
            {
                this.stream.Dispose();
            }
        }
    }
}
