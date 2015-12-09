namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Vector2F<{X},{Y}>")]
    public struct Vector2F
    {
        public static readonly Vector2F Zero = new Vector2F(0, 0);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Vector2F(float x, float y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2F(Vector2F that)
            : this()
        {
            this.X = that.X;
            this.Y = that.Y;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float X { get; set; }
        public float Y { get; set; }

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

        [JsonIgnore]
        public Vector2F Normalized
        {
            get
            {
                return this / this.Magnitude;
            }
        }

        [JsonIgnore]
        public float Angle
        {
            get
            {
                Vector2F normalized = this.Normalized;
                return (float)Math.Atan2(normalized.Y, normalized.X);
            }
        }

        public float this[int i]
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

        public static Vector2F operator +(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2F operator -(Vector2F v1, Vector2F v2)
        {
            return new Vector2F(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2F operator *(Vector2F v1, float s)
        {
            return new Vector2F(v1.X * s, v1.Y * s);
        }

        public static Vector2F operator /(Vector2F v1, float s)
        {
            return new Vector2F(v1.X / s, v1.Y / s);
        }

        public static bool operator >(Vector2F v1, Vector2F v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY > 0;
        }

        public static bool operator <(Vector2F v1, Vector2F v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY < 0;
        }

        public static bool operator ==(Vector2F v1, Vector2F v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector2F v1, Vector2F v2)
        {
            return !v1.Equals(v2);
        }
        
        public override bool Equals(object obj)
        {
            var typed = (Vector2F)obj;
            return typed.X.Equals(this.X) && typed.Y.Equals(this.Y);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y);
        }

        public float Dot(Vector2F v)
        {
            return (this.X * v.X) + (this.Y * v.Y);
        }

        public float DistanceTo(Vector2F v)
        {
            return Math.Abs((v - this).Magnitude);
        }

        public Vector2F DirectionTo(Vector2F v)
        {
            return new Vector2F(v.X - this.X, v.Y - this.Y).Normalized;
        }

        public float AngleTo(Vector2F v)
        {
            Vector2F res = v - this;
            return res.Angle;
        }
    }
}
