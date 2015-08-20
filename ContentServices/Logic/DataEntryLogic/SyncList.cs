namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    [DebuggerDisplay("{Value}")]
    public class SyncList<T, TN> : IList<TN>
        where T : IList<TN>
    {
        private T value;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SyncList()
            : this(default(T))
        {
        }

        public SyncList(T value)
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

        public TN this[int index]
        {
            get
            {
                return this.value[index];
            }

            set
            {
                this.value[index] = value;
                this.IsChanged = true;
            }
        }
        
        public IEnumerator<TN> GetEnumerator()
        {
            return this.value.GetEnumerator();
        }

        public override bool Equals(object obj)
        {
            var typed = (SyncList<T, TN>)obj;
            if (this.value == null && typed.Value == null)
            {
                return true;
            }

            if (this.value == null || typed.Value == null)
            {
                return false;
            }

            return typed.Value.Equals(this.value);
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

        public void Add(TN item)
        {
            this.value.Add(item);
            this.IsChanged = true;
        }

        public void Clear()
        {
            this.value.Clear();
            this.IsChanged = true;
        }

        public bool Contains(TN item)
        {
            return this.value.Contains(item);
        }

        public void CopyTo(TN[] array, int arrayIndex)
        {
            this.value.CopyTo(array, arrayIndex);
        }

        public bool Remove(TN item)
        {
            bool result = this.value.Remove(item);
            this.IsChanged = true;
            return result;
        }

        public int IndexOf(TN item)
        {
            return this.value.IndexOf(item);
        }

        public void Insert(int index, TN item)
        {
            this.value.Insert(index, item);
            this.IsChanged = true;
        }

        public void RemoveAt(int index)
        {
            this.value.RemoveAt(index);
            this.IsChanged = true;
        }
    }
}
