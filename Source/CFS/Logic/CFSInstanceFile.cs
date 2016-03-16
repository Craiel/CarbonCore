namespace CarbonCore.CFS.Logic
{
    using System.IO;
    
    using CarbonCore.Utils.IO;

    public class CFSInstanceFile : CFSInstance
    {
        private readonly FileStream stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CFSInstanceFile(CarbonFile file)
        {
            this.stream = file.OpenWrite();
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
                this.stream.Flush();
                this.stream.Dispose();
            }
        }
    }
}
