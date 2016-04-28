namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using CarbonCore.Utils;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Vector3D<{X},{Y},{Z}>")]
    public struct Vector3D : IComparable<Vector3D>
    {
        public static readonly Vector3D Zero = new Vector3D(0, 0, 0);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Vector3D(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Vector3D(Vector3D other)
            : this(other.X, other.Y, other.Z)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        [JsonIgnore]
        public bool IsZero
        {
            get
            {
                return Math.Abs(this.X) < float.Epsilon 
                    && Math.Abs(this.Y) < float.Epsilon
                    && Math.Abs(this.Z) < float.Epsilon;
            }
        }

        [JsonIgnore]
        public double LengthQ
        {
            get
            {
                return (this.X * this.X) + (this.Y * this.Y) + (this.Z * this.Z);
            }
        }

        [JsonIgnore]
        public double LengthVector
        {
            get
            {
                return Math.Sqrt(this.LengthQ);
            }
        }

        public double this[int i]
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

                    case 2:
                        {
                            return this.Z;
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

                    case 2:
                        {
                            this.Z = value;
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static bool operator >(Vector3D v1, Vector3D v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            int resultZ = v1.Z.CompareTo(v2.Z);
            if (resultX <= 0 && resultY <= 0 && resultZ <= 0)
            {
                return false;
            }

            return resultX + resultY + resultZ > 0;
        }

        public static bool operator <(Vector3D v1, Vector3D v2)
        {
            int resultX = v1.X.CompareTo(v2.X);
            int resultY = v1.Y.CompareTo(v2.Y);
            int resultZ = v1.Z.CompareTo(v2.Z);
            if (resultX <= 0 && resultY <= 0 && resultZ <= 0)
            {
                return false;
            }

            return resultX + resultY + resultZ < 0;
        }

        public static bool operator ==(Vector3D v1, Vector3D v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector3D v1, Vector3D v2)
        {
            return !v1.Equals(v2);
        }

        public static Vector3D operator +(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }

        public static Vector3D operator -(Vector3D v1, Vector3D v2)
        {
            return new Vector3D(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }

        public static Vector3D operator *(Vector3D v1, double s)
        {
            return new Vector3D(v1.X * s, v1.Y * s, v1.Z * s);
        }

        public static Vector3D operator /(Vector3D v1, double s)
        {
            return new Vector3D(v1.X / s, v1.Y / s, v1.Z / s);
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector3D)obj;
            return Math.Abs(typed.X - this.X) < double.Epsilon && Math.Abs(typed.Y - this.Y) < double.Epsilon && Math.Abs(typed.Z - this.Z) < double.Epsilon;
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y, this.Z);
        }

        public int CompareTo(Vector3D other)
        {
            return this.X.CompareTo(other.X) - this.Y.CompareTo(other.Y) - this.Z.CompareTo(other.Z);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2})", this.X, this.Y, this.Z);
        }

        public Vector3D Cross(Vector3D other)
        {
            return new Vector3D(
                (this.Y * other.Z) - (this.Z * other.Y),
                (this.Z * other.X) - (this.X * other.Z),
                (this.X * other.Y) - (this.Y * other.X));
        }

        public double Dot(Vector3D other)
        {
            return this.X * other.X + this.Y * other.Y + this.Z * other.Z;
        }

        public double Distance(Vector3D other)
        {
            return Math.Sqrt(this.DistanceSq(other.X, other.Y, other.Z));
        }

        public double DistanceSq(double toX, double toY, double toZ)
        {
            double x = this.X - toX;
            double y = this.Y - toY;
            double z = this.Z - toZ;
            return x * x + y * y + z * z;
        }

        public double DistanceSqToCenter(double xIn, double yIn, double zIn)
        {
            double x = this.X + 0.5d - xIn;
            double y = this.Y + 0.5d - yIn;
            double z = this.Z + 0.5d - zIn;
            return x * x + y * y + z * z;
        }

        public double DistanceSq(Vector3D to)
        {
            return this.DistanceSq(to.X, to.Y, to.Z);
        }

        public Vector3D SubstractReverse(Vector3D other)
        {
            return new Vector3D(other.X - this.X, other.Y - this.Y, other.Z - this.Z);
        }

        public Vector3D Normalize()
        {
            double length = this.LengthQ;
            return Math.Abs(length) < double.Epsilon ? Zero : new Vector3D(this.X / length, this.Y / length, this.Z / length);
        }

        public Vector3D Substract(Vector3D other)
        {
            return this.Substract(other.X, other.Y, other.Z);
        }

        public Vector3D Substract(double x, double y, double z)
        {
            return this.Add(-x, -y, -z);
        }

        public Vector3D Add(Vector3D other)
        {
            return this.Add(other.X, other.Y, other.Z);
        }

        public Vector3D Add(double x, double y, double z)
        {
            return new Vector3D(this.X + x, this.Y + y, this.Z + z);
        }

        public Vector3D RotatePitch(float pitch)
        {
            double cos = Math.Cos(pitch);
            double sin = Math.Sin(pitch);
            double x = this.X;
            double y = this.Y * cos + this.Z * sin;
            double z = this.Z * cos - this.X * sin;
            return new Vector3D(x, y, z);
        }

        public Vector3D RotateYaw(float yaw)
        {
            double cos = Math.Cos(yaw);
            double sin = Math.Sin(yaw);
            double x = this.X * cos + this.Z * sin;
            double y = this.Y;
            double z = this.Z * cos - this.X * sin;
            return new Vector3D(x, y, z);
        }

        // Returns a new vector with x value equal to the second parameter,
        // along the line between this vector and the passed in vector, or null if not possible
        public Vector3D? IntermediateWithXValue(Vector3D other, double value)
        {
            double x = other.X - this.X;
            double y = other.Y - this.Y;
            double z = other.Z - this.Z;

            if (x * x < double.Epsilon)
            {
                return null;
            }

            double result = (value - this.X) / x;
            if (result >= double.Epsilon && result <= 1.0d)
            {
                return new Vector3D(this.X + x * result, this.Y + y * result, this.Z + z * result);
            }

            return null;
        }

        public Vector3D? IntermediateWithYValue(Vector3D other, double value)
        {
            double x = other.X - this.X;
            double y = other.Y - this.Y;
            double z = other.Z - this.Z;

            if (y * y < double.Epsilon)
            {
                return null;
            }

            double result = (value - this.Y) / y;
            if (result >= double.Epsilon && result <= 1.0d)
            {
                return new Vector3D(this.X + x * result, this.Y + y * result, this.Z + z * result);
            }

            return null;
        }

        public Vector3D? IntermediateWithZValue(Vector3D other, double value)
        {
            double x = other.X - this.X;
            double y = other.Y - this.Y;
            double z = other.Z - this.Z;

            if (z * z < double.Epsilon)
            {
                return null;
            }

            double result = (value - this.Z) / z;
            if (result >= double.Epsilon && result <= 1.0d)
            {
                return new Vector3D(this.X + x * result, this.Y + y * result, this.Z + z * result);
            }

            return null;
        }
    }
}
