namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Collections;
    using System.Collections.Generic;

    public class SyncDictionary<T, TK, TV> : IDictionary<TK, TV>
        where T : IDictionary<TK, TV>
    {
        private T value;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncDictionary()
            : this(default(T))
        {
        }

        public SyncDictionary(T value)
        {
            this.value = value;
            this.IsChanged = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Value
        {
            get
            {
                return this.value;
            }

            set
            {
                if ((this.Value == null && value == null)
                    || (this.value != null && this.value.Equals(value)))
                {
                    return;
                }

                this.value = value;
                this.IsChanged = true;
            }
        }

        public bool IsChanged { get; private set; }

        public int Count
        {
            get
            {
                return this.value.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.value.IsReadOnly;
            }
        }

        public ICollection<TK> Keys
        {
            get
            {
                return this.value.Keys;
            }
        }

        public ICollection<TV> Values
        {
            get
            {
                return this.value.Values;
            }
        }

        public TV this[TK key]
        {
            get
            {
                return this.value[key];
            }

            set
            {
                this.value[key] = value;
                this.IsChanged = true;
            }
        }
        
        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return this.value.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            var typed = (SyncDictionary<T, TK, TV>)obj;
            if (this.value == null && typed.Value == null)
            {
                return true;
            }

            if (this.value == null || typed.Value == null)
            {
                return false;
            }

            return typed.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            if (this.value == null)
            {
                return 0;
            }

            return this.value.GetHashCode();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public void ResetChangeState(bool state = false)
        {
            this.IsChanged = state;
        }

        public void Add(KeyValuePair<TK, TV> item)
        {
            this.value.Add(item);
            this.IsChanged = true;
        }

        public void Clear()
        {
            this.value.Clear();
            this.IsChanged = true;
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            return this.value.Contains(item);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            this.value.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            bool result = this.value.Remove(item);
            this.IsChanged = true;
            return result;
        }

        public bool ContainsKey(TK key)
        {
            return this.value.ContainsKey(key);
        }

        public void Add(TK key, TV newValue)
        {
            this.value.Add(key, newValue);
            this.IsChanged = true;
        }

        public bool Remove(TK key)
        {
            bool result = this.value.Remove(key);
            this.IsChanged = true;
            return result;
        }

        public bool TryGetValue(TK key, out TV outValue)
        {
            return this.value.TryGetValue(key, out outValue);
        }
    }
}
