namespace CarbonCore.Utils.Collections
{
    using System;

    public class CircularBuffer<T>
    {
        private static readonly DataEntry Invalid = new DataEntry { Valid = false };

        private readonly DataEntry[] buffer;
        private readonly int length;
        private int nextFree;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CircularBuffer(int length)
        {
            this.buffer = new DataEntry[length];
            this.length = length;
            this.nextFree = 0;
            for (int k = 0; k < length; k++)
            {
                this.buffer[k].Valid = false;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool HaveValue(int index)
        {
            return this.IsIndexInRange(index) && this.buffer[this.IndexToId(index)].Valid;
        }

        public T Get(int index)
        {
            if (!this.HaveValue(index))
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.buffer[this.IndexToId(index)].Data;
        }

        public void Set(int index, T value)
        {
            if (this.IsIndexInRange(index))
            {
                this.buffer[this.IndexToId(index)] = new DataEntry(value);
                return;
            }

            if (index == this.nextFree)
            {
                this.Add(value);
                return;
            }

            int startIndex = this.nextFree;
            int stopIndex = Math.Min(index, this.nextFree + this.length) - 1;
            for (int k = startIndex; k <= stopIndex; k++)
            {
                this.buffer[this.IndexToId(k)] = Invalid;
            }

            this.buffer[this.IndexToId(index)] = new DataEntry(value);
            this.nextFree = index + 1;
        }

        public void Add(T value)
        {
            this.buffer[this.IndexToId(this.nextFree)] = new DataEntry(value);
            this.nextFree++;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool IsIndexInRange(int index)
        {
            return this.nextFree - this.length <= index && index < this.nextFree;
        }

        private int IndexToId(int index)
        {
            return index % this.length;
        }

        // -------------------------------------------------------------------
        // Data Struct
        // -------------------------------------------------------------------
        private struct DataEntry
        {
            public readonly T Data;

            public bool Valid;

            public DataEntry(T value)
            {
                this.Valid = true;
                this.Data = value;
            }
        }
    }
}
