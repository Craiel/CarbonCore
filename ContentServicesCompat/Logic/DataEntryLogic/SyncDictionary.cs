namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Diagnostics;

    public class SyncDictionary<T, TK, TV> : IDictionary<TK, TV>
        where T : IDictionary<TK, TV>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncDictionary()
            : this(Activator.CreateInstance<T>())
        {
        }

        public SyncDictionary(T value)
        {
            Diagnostic.Assert(value != null);

            this.Value = value;
            this.IsChanged = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Value { get; private set; }

        public bool IsChanged { get; private set; }

        public int Count
        {
            get
            {
                return this.Value.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.Value.IsReadOnly;
            }
        }

        public ICollection<TK> Keys
        {
            get
            {
                return this.Value.Keys;
            }
        }

        public ICollection<TV> Values
        {
            get
            {
                return this.Value.Values;
            }
        }

        public TV this[TK key]
        {
            get
            {
                return this.Value[key];
            }

            set
            {
                this.Value[key] = value;
                this.IsChanged = true;
            }
        }
        
        public IEnumerator<KeyValuePair<TK, TV>> GetEnumerator()
        {
            return this.Value.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            var typed = (SyncDictionary<T, TK, TV>)obj;
            if (this.Value == null && typed.Value == null)
            {
                return true;
            }

            if (this.Value == null || typed.Value == null)
            {
                return false;
            }

            return typed.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            if (this.Value == null)
            {
                return 0;
            }

            return this.Value.GetHashCode();
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
            this.Value.Add(item);
            this.IsChanged = true;
        }

        public void Clear()
        {
            this.Value.Clear();
            this.IsChanged = true;
        }

        public bool Contains(KeyValuePair<TK, TV> item)
        {
            return this.Value.Contains(item);
        }

        public void CopyTo(KeyValuePair<TK, TV>[] array, int arrayIndex)
        {
            this.Value.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TK, TV> item)
        {
            bool result = this.Value.Remove(item);
            this.IsChanged = true;
            return result;
        }

        public bool ContainsKey(TK key)
        {
            return this.Value.ContainsKey(key);
        }

        public void Add(TK key, TV newValue)
        {
            this.Value.Add(key, newValue);
            this.IsChanged = true;
        }

        public bool Remove(TK key)
        {
            bool result = this.Value.Remove(key);
            this.IsChanged = true;
            return result;
        }

        public bool TryGetValue(TK key, out TV outValue)
        {
            return this.Value.TryGetValue(key, out outValue);
        }
    }
}
