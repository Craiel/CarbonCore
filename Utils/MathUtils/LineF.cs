namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("LineF<{Start},{End}>")]
    public struct LineF
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public LineF(Vector2F start, Vector2F end)
            : this()
        {
            this.Start = start;
            this.End = end;
        }

        public LineF(float x1, float y1, float x2, float y2)
            : this(new Vector2F(x1, y1), new Vector2F(x2, y2))
        {
        }

        public LineF(LineF that)
            : this(that.Start, that.End)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public Vector2F Start { get; set; }
        public Vector2F End { get; set; }

        [JsonIgnore]
        public bool IsVertical
        {
            get
            {
                return Math.Abs(this.End.X) - Math.Abs(this.Start.X) < float.Epsilon;
            }
        }

        [JsonIgnore]
        public bool IsHorizontal
        {
            get
            {
                return Math.Abs(this.End.Y) - Math.Abs(this.Start.Y) < float.Epsilon;
            }
        }

        public RectF GetBounds()
        {
            var topLeft = new Vector2F(0, 0);
            var bottomRight = new Vector2F(0, 0);
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

            bottomRight = new Vector2F(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
            return new RectF(topLeft, bottomRight);
        }
        
        public override bool Equals(object other)
        {
            var typed = (LineF)other;

            return typed.Start.Equals(this.Start) && typed.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Start, this.End).GetHashCode();
        }
    }
}
