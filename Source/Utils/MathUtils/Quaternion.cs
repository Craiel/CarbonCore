namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Quaternion<{X},{Y},{Z},{W}>")]
    public struct Quaternion : IComparable<Quaternion>
    {
        public static readonly Quaternion Zero = new Quaternion(0, 0, 0, 0);

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Quaternion(float x, float y, float z, float w)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.W = w;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float W { get; private set; }

        [JsonIgnore]
        public bool IsZero
        {
            get
            {
                return Math.Abs(this.X) < float.Epsilon
                    && Math.Abs(this.Y) < float.Epsilon
                    && Math.Abs(this.Z) < float.Epsilon
                    && Math.Abs(this.W) < float.Epsilon;
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

                    case 2:
                        {
                            return this.Z;
                        }

                    case 3:
                        {
                            return this.W;
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

                    case 3:
                        {
                            this.W = value;
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static bool operator ==(Quaternion v1, Quaternion v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Quaternion v1, Quaternion v2)
        {
            return !v1.Equals(v2);
        }

        public override bool Equals(object obj)
        {
            var typed = (Quaternion)obj;
            return typed.X == this.X && typed.Y == this.Y && typed.Z == this.Z && typed.W == this.W;
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y, this.Z, this.W);
        }

        public int CompareTo(Quaternion other)
        {
            return this.X.CompareTo(other.X) - this.Y.CompareTo(other.Y) - this.Z.CompareTo(other.Z) - this.W.CompareTo(other.W);
        }

        public override string ToString()
        {
            return string.Format("({0},{1},{2},{3})", this.X, this.Y, this.Z, this.W);
        }
    }
}
