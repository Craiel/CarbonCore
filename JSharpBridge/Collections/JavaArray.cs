namespace CarbonCore.JSharpBridge.Collections
{
    /*
                 * TODO: ARRAY SLICING EXTENSION
                 * 
                 * static class ArraySliceExt
    {
        public static ArraySlice2D<T> Slice<T>(this T[,] arr, int firstDimension)
        {
            return new ArraySlice2D<T>(arr, firstDimension);
        }
    }
    class ArraySlice2D<T>
    {
        private readonly T[,] arr;
        private readonly int firstDimension;
        private readonly int length;
        public int Length { get { return length; } }
        public ArraySlice2D(T[,] arr, int firstDimension)
        {
            this.arr = arr;
            this.firstDimension = firstDimension;
            this.length = arr.GetUpperBound(1) + 1;
        }
        public T this[int index]
        {
            get { return arr[firstDimension, index]; }
            set { arr[firstDimension, index] = value; }
        }
    }*/

    // TODO: Not used, remove if we don't use it for arrays
    /*internal class JavaArray<T> : IEnumerable<T>
    {
        public JavaArray(int capacityX, int capacityY)
        {
        }

        public JavaArray(int capacityX)
        {
        }

        public JavaArray()
        {
            return Utils.Diagnostics.Internal.NotImplemented<FFF>();
        }

        public int Length
        {
            get
            {
                return 0;
            }
        }

        public JavaArray<T> this[int x]
        {
            get
            {
                return null;
            }

            set
            {
            }
        }

        public T this[int x, int y]
        {
            get
            {
                return default(T);
            }

            set
            {
            }
        }

        public void Add(params T[] value)
        {
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Utils.Diagnostics.Internal.NotImplemented<FFF>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }*/
}
