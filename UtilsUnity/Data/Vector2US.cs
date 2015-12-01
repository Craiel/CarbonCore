namespace CarbonCore.Utils.Unity.Data
{
    using System.Diagnostics;

    using UnityEngine;

    [DebuggerDisplay("{X}x{Y}")]
    public struct Vector2US
    {
        public static readonly Vector2US Zero = new Vector2US(0, 0);

        // ------------------------------------------------------------------- 
        // Constructor
        // ------------------------------------------------------------------- 
        public Vector2US(ushort x, ushort y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public Vector2US(Vector2 vector)
            : this((ushort)vector.x, (ushort)vector.y)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public ushort X { get; }

        public ushort Y { get; }

        public static bool operator ==(Vector2US x, Vector2US y)
        {
            return x.Equals(y);
        }

        public static bool operator !=(Vector2US x, Vector2US y)
        {
            return !x.Equals(y);
        }

        public static Vector2 operator -(Vector2US x, Vector2US y)
        {
            return new Vector2(x.X - y.X, x.Y - y.Y);
        }

        public static Vector2 operator +(Vector2US x, Vector2US y)
        {
            return new Vector2(x.X + y.X, x.Y + y.Y);
        }

        public override bool Equals(object obj)
        {
            var typed = (Vector2US)obj;
            return typed.X == this.X && typed.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }

        public override string ToString()
        {
            return $"{this.X}x{this.Y}";
        }

        public Vector2 ToVector2()
        {
            return new Vector2(this.X, this.Y);
        }
    }
}
