namespace CarbonCore.Utils.Unity.Data
{
    using System.Diagnostics;

    using CarbonCore.Utils.Compat;

    using UnityEngine;

    [DebuggerDisplay("{X}x{Y}")]
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

        public Vector2I(Vector2 vector)
            : this((int)vector.x, (int)vector.y)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public int X { get; private set; }

        public int Y { get; private set; }

        public static bool operator ==(Vector2I x, Vector2I y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Vector2I x, Vector2I y)
        {
            return !x.Equals(y);
        }

        public static Vector2I operator -(Vector2I x, Vector2I y)
        {
            return new Vector2I(x.X - y.X, x.Y - y.Y);
        }

        public static Vector2I operator +(Vector2I x, Vector2I y)
        {
            return new Vector2I(x.X + y.X, x.Y + y.Y);
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector2I)obj;
            return typed.X == this.X && typed.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.X, this.Y);
        }

        public override string ToString()
        {
            return string.Format("{0}x{1}", this.X, this.Y);
        }

        public Vector2 ToVector2()
        {
            return new Vector2(this.X, this.Y);
        }
    }
}
