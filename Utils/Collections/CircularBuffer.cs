namespace CarbonCore.Utils.Collections
{
    using System;

    public class CircularBuffer<T>
    {
        private struct Data
        {
            public static Data Invalid = new Data { valid = false };

            public Data(T value)
            {
                this.valid = true;
                this.data = value;
            }

            public bool valid;
            public T data;
        }

        Data[] buffer;
        int length;
        int nextFree;

        public CircularBuffer(int length)
        {
            this.buffer = new Data[length];
            this.length = length;
            this.nextFree = 0;
            for (int k = 0; k < length; k++)
            {
                this.buffer[k].valid = false;
            }
        }

        public bool HaveValue(int index)
        {
            return IsIndexInRange(index) &&
                   buffer[IndexToId(index)].valid;
        }

        public T Get(int index)
        {
            if (!HaveValue(index))
            {
                throw new ArgumentOutOfRangeException();
            }
            return buffer[IndexToId(index)].data;
        }

        public void Set(int index, T value)
        {
            if (IsIndexInRange(index))
            {
                buffer[IndexToId(index)] = new Data(value);
            }
            else if (index == nextFree)
            {
                Add(value);
            }
            else
            {
                int startIndex = nextFree;
                int stopIndex = Math.Min(index, nextFree + length) - 1;
                for (int k = startIndex; k <= stopIndex; k++)
                {
                    buffer[IndexToId(k)] = Data.Invalid;
                }

                buffer[IndexToId(index)] = new Data(value);
                nextFree = index + 1;
            }
        }

        public void Add(T value)
        {
            buffer[IndexToId(nextFree)] = new Data(value);
            nextFree++;
        }

        private bool IsIndexInRange(int index)
        {
            return nextFree - length <= index && index < nextFree;
        }

        private int IndexToId(int index)
        {
            return index % length;
        }
    }
}
