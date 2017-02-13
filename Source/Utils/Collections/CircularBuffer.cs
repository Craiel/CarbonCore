namespace CarbonCore.Utils.Collections
{
    using System;

    public class CircularBuffer<T>
    {
        private static readonly DataEntry Invalid = new DataEntry { Valid = false };

        private readonly DataEntry[] buffer;
        private readonly ulong length;
        private ulong nextFree;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CircularBuffer(ulong length)
        {
            this.buffer = new DataEntry[length];
            this.length = length;
            this.nextFree = 0;
            for (ulong k = 0; k < length; k++)
            {
                this.buffer[k].Valid = false;
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool HaveValue(ulong index)
        {
            return this.IsIndexInRange(index) && this.buffer[this.IndexToId(index)].Valid;
        }

        public T Get(ulong index)
        {
            if (!this.HaveValue(index))
            {
                throw new ArgumentOutOfRangeException();
            }

            return this.buffer[this.IndexToId(index)].Data;
        }

        public void Set(ulong index, T value)
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

            ulong startIndex = this.nextFree;
            ulong stopIndex = Math.Min(index, this.nextFree + this.length) - 1;
            for (ulong k = startIndex; k <= stopIndex; k++)
            {
                this.buffer[this.IndexToId(k)] = Invalid;
            }

            this.buffer[this.IndexToId(index)] = new DataEntry(value);
            this.nextFree = index + 1;
        }

        public ulong Add(T value)
        {
            ulong id = this.IndexToId(this.nextFree);
            this.buffer[id] = new DataEntry(value);
            this.nextFree++;
            return this.nextFree - 1;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool IsIndexInRange(ulong index)
        {
            return this.nextFree - this.length <= index && index < this.nextFree;
        }

        private ulong IndexToId(ulong index)
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
