namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("LineI<{Start},{End}>")]
    public struct LineI
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public LineI(Vector2I start, Vector2I end)
            : this()
        {
            this.Start = start;
            this.End = end;
        }

        public LineI(int x1, int y1, int x2, int y2)
            : this(new Vector2I(x1, y1), new Vector2I(x2, y2))
        {
        }

        public LineI(LineI that)
            : this(that.Start, that.End)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public Vector2I Start { get; set; }
        public Vector2I End { get; set; }

        [JsonIgnore]
        public bool IsVertical
        {
            get
            {
                return Math.Abs(this.End.X) - Math.Abs(this.Start.X) == 0;
            }
        }

        [JsonIgnore]
        public bool IsHorizontal
        {
            get
            {
                return Math.Abs(this.End.Y) - Math.Abs(this.Start.Y) == 0;
            }
        }

        public void GetBounds(out Vector2I topLeft, out Vector2I bottomRight)
        {
            RectF bounds = new LineF(this.Start.X, this.Start.Y, this.End.X, this.End.Y).GetBounds();

            topLeft = new Vector2I((int)bounds.Left, (int)bounds.Top);
            bottomRight = new Vector2I((int)bounds.Right, (int)bounds.Bottom);
        }
        
        public override bool Equals(object other)
        {
            var typed = (LineI)other;

            return typed.Start.Equals(this.Start) && typed.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.Start, this.End);
        }
    }
}
