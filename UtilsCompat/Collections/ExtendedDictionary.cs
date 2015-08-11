namespace CarbonCore.Utils.Compat.Collections
{
    using System.Collections;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts;

    public class ExtendedDictionary<T, TN> : IExtendedDictionary<T, TN>
    {
        private readonly IDictionary<T, TN> inner;

        private IDictionary<TN, T> innerReverse;

        private bool enableReverseLookup;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ExtendedDictionary()
        {
            this.inner = new Dictionary<T, TN>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Count
        {
            get
            {
                return this.inner.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.inner.IsReadOnly;
            }
        }

        public bool EnableReverseLookup
        {
            get
            {
                return this.enableReverseLookup;
            }

            set
            {
                if (this.enableReverseLookup != value)
                {
                    this.enableReverseLookup = value;
                    this.UpdateReverseCache();
                }
            }
        }

        public ICollection<T> Keys
        {
            get
            {
                return this.inner.Keys;
            }
        }

        public ICollection<TN> Values
        {
            get
            {
                return this.inner.Values;
            }
        }

        public virtual TN this[T key]
        {
            get
            {
                return this.inner[key];
            }

            set
            {
                this.inner[key] = value;
            }
        }

        public virtual T this[TN key]
        {
            get
            {
                return this.innerReverse[key];
            }

            set
            {
                this.innerReverse[key] = value;
            }
        }

        public IEnumerator<KeyValuePair<T, TN>> GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        public IEnumerator<KeyValuePair<TN, T>> GetReverseEnumerator()
        {
            System.Diagnostics.Trace.Assert(this.innerReverse != null);

            return this.innerReverse.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.inner.GetEnumerator();
        }

        public void Add(KeyValuePair<T, TN> item)
        {
            this.inner.Add(item);
            if (this.innerReverse != null)
            {
                this.innerReverse.Add(item.Value, item.Key);
            }
        }

        public void Add(KeyValuePair<TN, T> item)
        {
            System.Diagnostics.Trace.Assert(this.innerReverse != null);

            this.innerReverse.Add(item);
            this.inner.Add(item.Value, item.Key);
        }

        public void Clear()
        {
            this.inner.Clear();
            if (this.innerReverse != null)
            {
                this.innerReverse.Clear();
            }
        }

        public bool Contains(KeyValuePair<T, TN> item)
        {
            return this.inner.Contains(item);
        }

        public bool Contains(KeyValuePair<TN, T> item)
        {
            System.Diagnostics.Trace.Assert(this.innerReverse != null);

            return this.innerReverse.Contains(item);
        }

        public void CopyTo(KeyValuePair<T, TN>[] array, int arrayIndex)
        {
            this.inner.CopyTo(array, arrayIndex);
        }

        public void CopyTo(KeyValuePair<TN, T>[] array, int arrayIndex)
        {
            System.Diagnostics.Trace.Assert(this.innerReverse != null);

            this.innerReverse.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<T, TN> item)
        {
            if (this.innerReverse != null)
            {
                if (!this.innerReverse.Remove(item.Value))
                {
                    return false;
                }
            }

            return this.inner.Remove(item);
        }

        public bool Remove(KeyValuePair<TN, T> item)
        {
            System.Diagnostics.Trace.Assert(this.innerReverse != null);

            if (!this.inner.Remove(item.Value))
            {
                return false;
            }

            return this.innerReverse.Remove(item.Key);
        }
        
        public bool ContainsKey(T key)
        {
            return this.inner.ContainsKey(key);
        }

        public bool ContainsValue(TN value)
        {
            System.Diagnostics.Trace.Assert(this.innerReverse != null);

            return this.innerReverse.ContainsKey(value);
        }

        public void Add(T key, TN value)
        {
            this.inner.Add(key, value);
            if (this.innerReverse != null)
            {
                this.innerReverse.Add(value, key);
            }
        }
        
        public bool Remove(T key)
        {
            if (this.innerReverse != null)
            {
                if (!this.innerReverse.Remove(this.inner[key]))
                {
                    return false;
                }
            }

            return this.inner.Remove(key);
        }

        public bool TryGetValue(T key, out TN value)
        {
            return this.inner.TryGetValue(key, out value);
        }

        public bool TryGetKey(TN value, out T key)
        {
            System.Diagnostics.Trace.Assert(this.innerReverse != null);

            return this.innerReverse.TryGetValue(value, out key);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void UpdateReverseCache()
        {
            if (!this.enableReverseLookup)
            {
                this.innerReverse = null;
                return;
            }

            this.innerReverse = new Dictionary<TN, T>();
            foreach (KeyValuePair<T, TN> pair in this.inner)
            {
                this.innerReverse.Add(pair.Value, pair.Key);
            }
        }
    }
}
