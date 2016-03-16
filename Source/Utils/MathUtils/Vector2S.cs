namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Vector2S<{X},{Y}>")]
    public struct Vector2S
    {
        public static readonly Vector2S Zero = new Vector2S(0, 0);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Vector2S(short x, short y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2S(Vector2S that)
            : this()
        {
            this.X = that.X;
            this.Y = that.Y;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public short X { get; set; }
        public short Y { get; set; }

        [JsonIgnore]
        public bool IsZero
        {
            get
            {
                return this.X == 0 && this.Y == 0;
            }
        }

        [JsonIgnore]
        public float LengthQ
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y);
            }
        }

        [JsonIgnore]
        public float Magnitude
        {
            get
            {
                return (float)Math.Sqrt(this.LengthQ);
            }
        }

        public short this[int i]
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

        public static bool operator >(Vector2S v1, Vector2S v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY > 0;
        }

        public static bool operator <(Vector2S v1, Vector2S v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY < 0;
        }

        public static bool operator ==(Vector2S v1, Vector2S v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector2S v1, Vector2S v2)
        {
            return !v1.Equals(v2);
        }

        public static Vector2S operator +(Vector2S v1, Vector2S v2)
        {
            return new Vector2S((short)(v1.X + v2.X), (short)(v1.Y + v2.Y));
        }
        
        public static Vector2S operator -(Vector2S v1, Vector2S v2)
        {
            return new Vector2S((short)(v1.X - v2.X), (short)(v2.Y - v2.Y));
        }

        public static Vector2S operator *(Vector2S v1, short s)
        {
            return new Vector2S((short)(v1.X * s), (short)(v1.Y * s));
        }

        public static Vector2S operator /(Vector2S v1, short s)
        {
            return new Vector2S((short)(v1.X / s), (short)(v1.Y / s));
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", this.X, this.Y);
        }

        public short DistanceX(Vector2S that)
        {
            return (short)(this.X - that.X);
        }

        public short DistanceY(Vector2S that)
        {
            return (short)(this.Y - that.Y);
        }

        public short Distance(Vector2S that)
        {
            return (short)Math.Sqrt(((this.X - that.X) * (this.X - that.X)) + ((this.Y - that.Y) * (this.Y - that.Y)));
        }

        public Vector2F ToVector2F()
        {
            return new Vector2F(this.X, this.Y);
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector2S)obj;
            return typed.X.Equals(this.X) && typed.Y.Equals(this.Y);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y);
        }

        public Vector2S Min(Vector2S target)
        {
            return new Vector2S(Math.Min(this.X, target.X), Math.Min(this.Y, target.Y));
        }

        public Vector2S Max(Vector2S target)
        {
            return new Vector2S(Math.Max(this.X, target.X), Math.Max(this.Y, target.Y));
        }
    }
}
