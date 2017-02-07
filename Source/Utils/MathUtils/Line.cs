namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;
    using Microsoft.Xna.Framework;
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("Line<{Start},{End}>")]
    public struct Line
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public Line(Point start, Point end)
            : this()
        {
            this.Start = start;
            this.End = end;
        }

        public Line(int x1, int y1, int x2, int y2)
            : this(new Point(x1, y1), new Point(x2, y2))
        {
        }

        public Line(Line that)
            : this(that.Start, that.End)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public Point Start { get; set; }

        public Point End { get; set; }

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

        public Rectangle GetBounds()
        {
            var topLeft = new Point(0, 0);
            var bottomRight = new Point(0, 0);
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

            bottomRight = new Point(bottomRight.X - topLeft.X, bottomRight.Y - topLeft.Y);
            return new Rectangle(topLeft, bottomRight);
        }
        
        public override bool Equals(object other)
        {
            var typed = (Line)other;

            return typed.Start.Equals(this.Start) && typed.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.Start, this.End);
        }
    }
}
