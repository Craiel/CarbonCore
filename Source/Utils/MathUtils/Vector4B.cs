namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Vector4B<{A},{B},{C},{D}>")]
    public struct Vector4B
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Vector4B(byte a, byte b, byte c, byte d)
        {
            this.A = a;
            this.B = b;
            this.C = c;
            this.D = d;
        }

        public Vector4B(Vector4B other)
            : this(other.A, other.B, other.C, other.D)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public byte A { get; private set; }
        public byte B { get; private set; }
        public byte C { get; private set; }
        public byte D { get; private set; }

        public byte this[int i]
        {
            get
            {
                switch (i)
                {
                    case 0:
                        {
                            return this.A;
                        }

                    case 1:
                        {
                            return this.B;
                        }

                    case 2:
                        {
                            return this.C;
                        }

                    case 3:
                        {
                            return this.D;
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
                            this.A = value;
                            break;
                        }

                    case 1:
                        {
                            this.B = value;
                            break;
                        }

                    case 2:
                        {
                            this.C = value;
                            break;
                        }

                    case 3:
                        {
                            this.D = value;
                            break;
                        }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public static bool operator >(Vector4B v1, Vector4B v2)
        {
            int resultA = v1.A.CompareTo(v2.A);
            int resultB = v1.B.CompareTo(v2.B);
            int resultC = v1.C.CompareTo(v2.C);
            int resultD = v1.D.CompareTo(v2.D);
            if (resultA <= 0 && resultB <= 0 && resultC <= 0 && resultD <= 0)
            {
                return false;
            }

            return resultA + resultB + resultC + resultD > 0;
        }

        public static bool operator <(Vector4B v1, Vector4B v2)
        {
            int resultA = v1.A.CompareTo(v2.A);
            int resultB = v1.B.CompareTo(v2.B);
            int resultC = v1.C.CompareTo(v2.C);
            int resultD = v1.D.CompareTo(v2.D);
            if (resultA <= 0 && resultB <= 0 && resultC <= 0 && resultD <= 0)
            {
                return false;
            }

            return resultA + resultB + resultC + resultD < 0;
        }

        public static bool operator ==(Vector4B v1, Vector4B v2)
        {
            return v1.Equals(v2);
        }

        public static bool operator !=(Vector4B v1, Vector4B v2)
        {
            return !v1.Equals(v2);
        }

        public static Vector4B operator +(Vector4B v1, Vector4B v2)
        {
            return new Vector4B((byte)(v1.A + v2.A), (byte)(v1.B + v2.B), (byte)(v1.C + v2.C), (byte)(v1.D + v2.D));
        }

        public static Vector4B operator -(Vector4B v1, Vector4B v2)
        {
            return new Vector4B((byte)(v1.A - v2.A), (byte)(v1.B - v2.B), (byte)(v1.C - v2.C), (byte)(v1.D - v2.D));
        }

        public static Vector4B operator *(Vector4B v1, byte s)
        {
            return new Vector4B((byte)(v1.A * s), (byte)(v1.B * s), (byte)(v1.C * s), (byte)(v1.D * s));
        }

        public static Vector4B operator /(Vector4B v1, byte s)
        {
            return new Vector4B((byte)(v1.A / s), (byte)(v1.B / s), (byte)(v1.C / s), (byte)(v1.D / s));
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector4B)obj;
            return typed.A == this.A && typed.B == this.B && typed.C == this.C && typed.D == this.D;
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.A, this.B, this.C, this.D);
        }
    }
}
