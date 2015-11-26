namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Compat.Diagnostics.Metrics;
    using CarbonCore.Utils.Unity.Contracts;
    using CarbonCore.Utils.Unity.Data;

    using UnityEngine;

    public class ResourceStreamRequest : IResourceRequest
    {
        private readonly WWW stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStreamRequest(ResourceKey key)
        {
            this.Key = key;

            this.Metric = Diagnostic.BeginTimeMeasure();

            this.stream = new WWW(key.Path);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceKey Key { get; private set; }
        
        public MetricTime Metric { get; private set; }

        public bool IsDone
        {
            get
            {
                return this.stream.isDone;
            }
        }

        public byte[] GetData()
        {
            Diagnostic.Assert(this.IsDone);

            if (!string.IsNullOrEmpty(this.stream.error))
            {
                Diagnostic.Error("ResourceStreamRequest had errors: {0}", this.stream.error);
                return null;
            }

            return this.stream.bytes;
        }
    }
}
