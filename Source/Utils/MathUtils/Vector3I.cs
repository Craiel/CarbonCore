namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using CarbonCore.Utils;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Vector3I<{X},{Y},{Z}>")]
    public struct Vector3I : IComparable<Vector3I>
    {
        public static readonly Vector3I Zero = new Vector3I(0, 0, 0);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Vector3I(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3I(double x, double y, double z)
            : this((int)Math.Floor(x), (int)Math.Floor(y), (int)Math.Floor(z))
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int X { get; private set; }
        public int Y { get; private set; }
        public int Z { get; private set; }

        [JsonIgnore]
        public bool IsZero
        {
            get
            {
                return this.X == 0 && this.Y == 0 && this.Z == 0;
            }
        }

        [JsonIgnore]
        public float LengthQ
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
            }
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector3I)obj;
            return typed.X == this.X && typed.Y == this.Y && typed.Z == this.Z;
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y, this.Z);
        }

        public int CompareTo(Vector3I other)
        {
            return this.X.CompareTo(other.X) - this.Y.CompareTo(other.Y) - this.Z.CompareTo(other.Z);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", this.X, this.Y, this.Z);
        }

        public Vector3I Cross(Vector3I other)
        {
            return new Vector3I(
                (this.Y * other.Z) - (this.Z * other.Y),
                (this.Z * other.X) - (this.X * other.Z),
                (this.X * other.Y) - (this.Y * other.X));
        }

        public double Distance(int x, int y, int z)
        {
            return Math.Sqrt(this.DistanceSq(x, y, z));
        }

        public double DistanceSq(double toX, double toY, double toZ)
        {
            double d0 = this.X - toX;
            double d1 = this.Y - toY;
            double d2 = this.Z - toZ;
            return d0 * d0 + d1 * d1 + d2 * d2;
        }

        public double DistanceSqToCenter(double xIn, double yIn, double zIn)
        {
            double d0 = this.X + 0.5d - xIn;
            double d1 = this.Y + 0.5d - yIn;
            double d2 = this.Z + 0.5d - zIn;
            return d0 * d0 + d1 * d1 + d2 * d2;
        }

        public double DistanceSq(Vector3I to)
        {
            return this.DistanceSq(to.X, to.Y, to.Z);
        }
    }
}
