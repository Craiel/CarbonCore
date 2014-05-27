namespace CarbonCore.JSharpBridge.Collections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    // Java-style wrapper around IEnumerator
    public interface IIterator
    {
    }

    public class SetIterator<T> : Iterator, IIterator
        where T : class
    {
        private readonly ISet<T> host;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SetIterator(ISet<T> host, IEnumerator enumerator)
            : base(enumerator)
        {
            this.host = host;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Remove()
        {
            this.host.Remove((T)this.Current);
        }
    }

    public class ListIterator : Iterator
    {
        private readonly IList host;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ListIterator(IList host, IEnumerator enumerator)
            : base(enumerator)
        {
            this.host = host;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Remove()
        {
            this.host.Remove(this.Current);
        }
    }

    public class Iterator : IIterator
    {
        private readonly IEnumerator enumerator;

        private object current;
        private object next;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Iterator(IEnumerator enumerator)
        {
            this.enumerator = enumerator;
            this.current = enumerator.Current;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool AtEnd { get; private set; }

        public bool HasNext()
        {
            if (this.AtEnd)
            {
                return false;
            }

            if (this.next == null)
            {
                return this.LoadNext();
            }

            return this.next != null;
        }

        public object Next()
        {
            if (this.next == null)
            {
                if (!this.LoadNext())
                {
                    return null;
                }
            }

            this.current = this.next;
            this.next = null;
            return this.current;
        }

        public virtual void Remove()
        {
            throw new InvalidOperationException();
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected object Current
        {
            get
            {
                return this.current;
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool LoadNext()
        {
            this.next = null;

            // Try to advance
            if (!this.enumerator.MoveNext())
            {
                this.AtEnd = true;
                return false;
            }

            // Set the current
            this.next = this.enumerator.Current;
            return true;
        }
    }
}
