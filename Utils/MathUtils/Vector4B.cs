namespace CarbonCore.Utils.MathUtils
{
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
