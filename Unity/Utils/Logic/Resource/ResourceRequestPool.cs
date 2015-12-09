namespace CarbonCore.Utils.Unity.Logic.Resource
{
    using System.Collections.Generic;
    using System.Linq;

    using CarbonCore.Utils.Unity.Contracts;

    public class ResourceRequestPool<T> where T : class, IResourceRequest
    {
        private readonly T[] requests;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResourceRequestPool(int size)
        {
            this.requests = new T[size];
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

        public IList<T> GetFinishedRequests()
        {
            if (this.requests.Any(x => x != null && x.IsDone))
            {
                IList<T> results = new List<T>();
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

        public void AddRequest(T request)
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

        public T GetFirstActiveRequest()
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
