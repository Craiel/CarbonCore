namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("LineL<{Start},{End}>")]
    public struct LineL
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public LineL(Vector2L start, Vector2L end)
            : this()
        {
            this.Start = start;
            this.End = end;
        }

        public LineL(long x1, long y1, long x2, long y2)
            : this(new Vector2L(x1, y1), new Vector2L(x2, y2))
        {
        }

        public LineL(LineL that)
            : this(that.Start, that.End)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public Vector2L Start { get; set; }
        public Vector2L End { get; set; }

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

        public RectL GetBounds()
        {
            var topLeft = new Vector2L(0, 0);
            var bottomRight = new Vector2L(0, 0);
            if (this.IsHorizontal)
            {
                topLeft.X = this.Start.X;
                bottomRight.X = this.Start.X;
            }
            else
            {
                if (this.Start.X < this.End.X)
                {
                    topLeft.X = this.Start.X;
                    bottomRight.X = this.End.X;
                }
                else
                {
                    topLeft.X = this.End.X;
                    bottomRight.X = this.Start.X;
                }
            }

            if (this.IsVertical)
            {
                topLeft.Y = this.Start.Y;
                bottomRight.Y = this.Start.Y;
            }
            else
            {
                if (this.Start.Y < this.End.Y)
                {
                    topLeft.Y = this.Start.Y;
                    bottomRight.Y = this.End.Y;
                }
                else
                {
                    topLeft.Y = this.End.Y;
                    bottomRight.Y = this.Start.Y;
                }
            }

            bottomRight = new Vector2L(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
            return new RectL(topLeft, bottomRight);
        }
        
        public override bool Equals(object other)
        {
            var typed = (LineL)other;

            return typed.Start.Equals(this.Start) && typed.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Start, this.End).GetHashCode();
        }
    }
}
