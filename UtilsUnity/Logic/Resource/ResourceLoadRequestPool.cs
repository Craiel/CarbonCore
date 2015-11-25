namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System.Collections.Generic;
    using System.Linq;

    public class ResourceLoadRequestPool
    {
        private readonly ResourceLoadRequest[] requests;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceLoadRequestPool(int size)
        {
            this.requests = new ResourceLoadRequest[size];
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Size
        {
            get
            {
                return this.requests.Length;
            }
        }

        public int ActiveRequestCount { get; private set; }

        public IList<ResourceLoadRequest> GetFinishedRequests()
        {
            if (this.requests.Any(x => x != null && x.IsDone))
            {
                IList<ResourceLoadRequest> results = new List<ResourceLoadRequest>();
                for (var i = 0; i < this.requests.Length; i++)
                {
                    if (this.requests[i] == null || !this.requests[i].IsDone)
                    {
                        continue;
                    }

                    results.Add(this.requests[i]);
                    this.requests[i] = null;
                    this.ActiveRequestCount--;
                }

                return results;
            }

            return null;
        }

        public bool HasFreeSlot()
        {
            return this.requests.Count(x => x == null) > 0;
        }

        public bool HasPendingRequests()
        {
            return this.requests.Any(x => x != null);
        }

        public void AddRequest(ResourceLoadRequest request)
        {
            for (var i = 0; i < this.requests.Length; i++)
            {
                if (this.requests[i] == null)
                {
                    this.requests[i] = request;
                    this.ActiveRequestCount++;
                    return;
                }
            }
        }

        public ResourceLoadRequest GetFirstActiveRequest()
        {
            for (var i = 0; i < this.requests.Length; i++)
            {
                if (this.requests[i] != null)
                {
                    return this.requests[i];
                }
            }

            return null;
        }
    }
}
