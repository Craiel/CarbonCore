namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Vector2L<{X},{Y}>")]
    public struct Vector2L
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Vector2L(long x, long y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2L(Vector2L that)
            : this()
        {
            this.X = that.X;
            this.Y = that.Y;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public long X { get; set; }
        public long Y { get; set; }

        public long this[int i]
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

        public static bool operator >(Vector2L v1, Vector2L v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY > 0;
        }

        public static bool operator <(Vector2L v1, Vector2L v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY < 0;
        }

        public static bool operator ==(Vector2L v1, Vector2L v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector2L v1, Vector2L v2)
        {
            return !v1.Equals(v2);
        }

        public static Vector2L operator +(Vector2L v1, Vector2L v2)
        {
            return new Vector2L(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2L operator -(Vector2L v1, Vector2L v2)
        {
            var vt = new Vector2L(v1);
            vt.X -= v2.X;
            vt.Y -= v2.Y;
            return vt;
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", this.X, this.Y);
        }

        public long DistanceX(Vector2L that)
        {
            return this.X - that.X;
        }

        public long DistanceY(Vector2L that)
        {
            return this.Y - that.Y;
        }

        public long Distance(Vector2L that)
        {
            return (long)Math.Sqrt(((this.X - that.X) * (this.X - that.X)) + ((this.Y - that.Y) * (this.Y - that.Y)));
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector2L)obj;
            return typed.X.Equals(this.X) && typed.Y.Equals(this.Y);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y);
        }
        
        public Vector2L Min(Vector2L target)
        {
            return new Vector2L(Math.Min(this.X, target.X), Math.Min(this.Y, target.Y));
        }

        public Vector2L Max(Vector2L target)
        {
            return new Vector2L(Math.Max(this.X, target.X), Math.Max(this.Y, target.Y));
        }
    }
}
