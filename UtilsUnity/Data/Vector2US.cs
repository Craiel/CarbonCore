namespace CarbonCore.Utils.Unity.Data
{
    public struct Vector2US
    {
        public static readonly Vector2US Zero = new Vector2US(0, 0);

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public Vector2US(ushort x, ushort y)
            : this()
        {
            this.X = x;
            this.Y = y;
        }

        public ushort X { get; private set; }

        public ushort Y { get; private set; }

        public override bool Equals(object obj)
        {
            var typed = (Vector2US)obj;
            return typed.X == this.X && typed.Y == this.Y;
        }

        public override int GetHashCode()
        {
            return this.X ^ this.Y;
        }
    }
}
