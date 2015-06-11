namespace CarbonCore.Utils.Compat
{
    using System.Collections.Generic;

    // Simple limited list which discards oldest entries while still allowing for sequential and random access
    public class LimitedList<T>
    {
        private readonly List<T> list;

        public LimitedList(int limit)
        {
            this.list = new List<T>(limit);
            this.Limit = limit;
        }

        public int Limit { get; private set; }

        public void Add(T item)
        {
            lock (this.list)
            {
                if (this.list.Count >= this.Limit)
                {
                    this.list.RemoveAt(0);
                }

                this.list.Add(item);
            }
        }

        public IList<T> GetState()
        {
            return new List<T>(this.list);
        }
    }
}
