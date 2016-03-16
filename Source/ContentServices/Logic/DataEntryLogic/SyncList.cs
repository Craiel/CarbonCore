namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.Utils.Diagnostics;

    [DebuggerDisplay("{Value}")]
    public class SyncList<T, TN> : IList<TN>
        where T : IList<TN>
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncList()
            : this(Activator.CreateInstance<T>())
        {
        }

        public SyncList(T value)
        {
            Diagnostic.Assert(value != null);

            this.Value = value;
            this.IsChanged = true;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public T Value { get; private set; }

        public virtual bool IsChanged { get; private set; }

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

        public TN this[int index]
        {
            get
            {
                return this.Value[index];
            }

            set
            {
                this.Value[index] = value;
                this.IsChanged = true;
            }
        }
        
        public IEnumerator<TN> GetEnumerator()
        {
            return this.Value.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            var typed = (SyncList<T, TN>)obj;
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

        public virtual void ResetChangeState(bool state = false)
        {
            this.IsChanged = state;
        }

        public void Add(TN item)
        {
            this.Value.Add(item);
            this.IsChanged = true;
        }

        public void Clear()
        {
            if (this.Value.Count <= 0)
            {
                // Avoid triggering change state if not needed
                return;
            }

            this.Value.Clear();
            this.IsChanged = true;
        }

        public bool Contains(TN item)
        {
            return this.Value.Contains(item);
        }

        public void CopyTo(TN[] array, int arrayIndex)
        {
            this.Value.CopyTo(array, arrayIndex);
        }

        public bool Remove(TN item)
        {
            bool result = this.Value.Remove(item);
            this.IsChanged = true;
            return result;
        }

        public int IndexOf(TN item)
        {
            return this.Value.IndexOf(item);
        }

        public void Insert(int index, TN item)
        {
            this.Value.Insert(index, item);
            this.IsChanged = true;
        }

        public void RemoveAt(int index)
        {
            this.Value.RemoveAt(index);
            this.IsChanged = true;
        }
    }
}
