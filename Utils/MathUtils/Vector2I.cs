namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Vector2I<{X},{Y}>")]
    public struct Vector2I
    {
        public static readonly Vector2I Zero = new Vector2I(0, 0);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Vector2I(int x, int y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2I(Vector2I that)
            : this()
        {
            this.X = that.X;
            this.Y = that.Y;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int X { get; set; }
        public int Y { get; set; }

        public int this[int i]
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

        public static bool operator >(Vector2I v1, Vector2I v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY > 0;
        }

        public static bool operator <(Vector2I v1, Vector2I v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            if (resultX <= 0 && resultY <= 0)
            {
                return false;
            }

            return resultX + resultY < 0;
        }

        public static bool operator ==(Vector2I v1, Vector2I v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector2I v1, Vector2I v2)
        {
            return !v1.Equals(v2);
        }

        public static Vector2I operator +(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X + v2.X, v1.Y + v2.Y);
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", this.X, this.Y);
        }

        public int DistanceX(Vector2I that)
        {
            return this.X - that.X;
        }

        public int DistanceY(Vector2I that)
        {
            return this.Y - that.Y;
        }

        public int Distance(Vector2I that)
        {
            return (int)Math.Sqrt(((this.X - that.X) * (this.X - that.X)) + ((this.Y - that.Y) * (this.Y - that.Y)));
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector2I)obj;
            return typed.X.Equals(this.X) && typed.Y.Equals(this.Y);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y);
        }
        
        public Vector2I Min(Vector2I target)
        {
            return new Vector2I(Math.Min(this.X, target.X), Math.Min(this.Y, target.Y));
        }

        public Vector2I Max(Vector2I target)
        {
            return new Vector2I(Math.Max(this.X, target.X), Math.Max(this.Y, target.Y));
        }
    }
}
