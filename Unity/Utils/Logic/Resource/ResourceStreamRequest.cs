namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Diagnostics.Metrics;
    using CarbonCore.Utils.Unity.Contracts;

    using UnityEngine;

    public class ResourceStreamRequest : IResourceRequest
    {
        private readonly WWW stream;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceStreamRequest(ResourceLoadInfo info)
        {
            this.Info = info;

            this.Metric = Diagnostic.BeginTimeMeasure();

            this.stream = new WWW(info.Key.Path);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ResourceLoadInfo Info { get; private set; }
        
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
