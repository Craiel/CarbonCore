namespace CarbonCore.Unity.Utils.Logic.Resource
{
    using CarbonCore.Unity.Utils.Contracts;

    using NLog;

    using UnityEngine;
    
    public class ResourceStreamRequest : IResourceRequest
    {
        private static readonly NLog.Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly WWW stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStreamRequest(ResourceLoadInfo info)
        {
            this.Info = info;
            
            this.stream = new WWW(info.Key.Path);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceLoadInfo Info { get; private set; }
        
        public bool IsDone
        {
            get
            {
                return this.stream.isDone;
            }
        }

        public byte[] GetData()
        {
            Debug.Assert(this.IsDone);

            if (!string.IsNullOrEmpty(this.stream.error))
            {
                Logger.Error("ResourceStreamRequest had errors: {0}", this.stream.error);
                return null;
            }

            return this.stream.bytes;
        }
    }
}
