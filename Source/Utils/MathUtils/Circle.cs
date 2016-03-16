namespace CarbonCore.Utils.MathUtils
{
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Circle<{Position}, {Radius}>")]
    public class Circle
    {
        // ------------------------------------------------------------------- 
        // Constructor
        // ------------------------------------------------------------------- 
        public Circle(long x, long y, int r)
            : this(new Vector2L(x, y), r)
        {
        }

        public Circle(Vector2L pos, int radius)
        {
            this.Position = pos;
            this.Radius = radius;
        }

        public Circle(Circle that)
            : this(that.Position, that.Radius)
        {
        }

        // ------------------------------------------------------------------- 
        // Public
        // ------------------------------------------------------------------- 
        public Vector2L Position { get; set; }

        public int Radius { get; set; }

        public override bool Equals(object other)
        {
            var typed = other as Circle;
            if (typed == null)
            {
                return false;
            }

            return typed.Position.Equals(this.Position) && typed.Radius.Equals(this.Radius);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.Position, this.Radius);
        }

        public RectL GetBounds()
        {
            var pos = new Vector2L(this.Position.X - this.Radius, this.Position.Y - this.Radius);
            return new RectL(pos, new Vector2L(this.Radius * 2, this.Radius * 2));
        }
    }
}
