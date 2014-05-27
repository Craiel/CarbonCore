namespace CarbonCore.UtilsSDL.Math
{
    using System;

    public struct Vector2<T>
        where T : struct, IComparable
    {
        public Vector2(T x, T y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2(Vector2<T> that)
            : this()
        {
            this.X = that.X;
            this.Y = that.Y;
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public T X { get; set; }
        public T Y { get; set; }

        public T this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        {
                            return this.X;
                        }

                    case 1:
                        {
                            return this.Y;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set 
            {
                switch (i)
                {
                    case 0:
                        {
                            this.X = value;
                            break;
                        }

                    case 1:
                        {
                            this.Y = value;
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static bool operator >(Vector2<T> v1, Vector2<T> v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY > 0;
        }

        public static bool operator <(Vector2<T> v1, Vector2<T> v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY < 0;
        }

        public static bool operator ==(Vector2<T> v1, Vector2<T> v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector2<T> v1, Vector2<T> v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector2<T>)obj;
            return typed.X.Equals(this.X) && typed.Y.Equals(this.Y);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.X, this.Y).GetHashCode();
        }
    }
}
