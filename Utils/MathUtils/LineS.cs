namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("LineS<{Start},{End}>")]
    public struct LineS
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public LineS(Vector2S start, Vector2S end)
            : this()
        {
            this.Start = start;
            this.End = end;
        }

        public LineS(short x1, short y1, short x2, short y2)
            : this(new Vector2S(x1, y1), new Vector2S(x2, y2))
        {
        }

        public LineS(LineS that)
            : this(that.Start, that.End)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public Vector2S Start { get; set; }
        public Vector2S End { get; set; }

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

        public RectS GetBounds()
        {
            RectF rect = new LineF(this.Start.X, this.Start.Y, this.End.X, this.End.Y).GetBounds();
            return new RectS((short)rect.Left, (short)rect.Top, (short)rect.Width, (short)rect.Height);
        }
        
        public override bool Equals(object other)
        {
            var typed = (LineS)other;

            return typed.Start.Equals(this.Start) && typed.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Start, this.End).GetHashCode();
        }
    }
}
