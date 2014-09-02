namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("RectL<{Left},{Top} - {Right},{Bottom}>")]
    public struct RectL
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public RectL(long x, long y, long w, long h)
            : this()
        {
            this.Top = y;
            this.Left = x;
            this.Bottom = y + h;
            this.Right = x + w;
        }

        public RectL(RectL that)
            : this(that.LeftTop, that.RightBottom)
        {
        }

        public RectL(Vector2L pos, Vector2L size)
            : this(pos.X, pos.Y, size.X, size.Y)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // -------------------------------------------------------------------
        public long Top { get; set; }
        public long Left { get; set; }
        public long Bottom { get; set; }
        public long Right { get; set; }

        [JsonIgnore]
        public Vector2L LeftTop
        {
            get
            {
                return new Vector2L(this.Left, this.Top);
            }
        }

        [JsonIgnore]
        public Vector2L RightBottom
        {
            get
            {
                return new Vector2L(this.Right, this.Bottom);
            }
        }

        [JsonIgnore]
        public long Width
        {
            get
            {
                return Math.Abs(this.Left - this.Right);
            }
        }

        [JsonIgnore]
        public long Height
        {
            get
            {
                return Math.Abs(this.Top - this.Bottom);
            }
        }

        [JsonIgnore]
        public Vector2L Size
        {
            get
            {
                return new Vector2L(this.Width, this.Height);
            }
        }

        [JsonIgnore]
        public Vector2L Center
        {
            get
            {
                return new Vector2L(this.Left + (this.Right / 2), this.Top + (this.Bottom / 2));
            }
        }

        public bool Contains(Vector2L p)
        {
            return p.X >= this.Left
            && p.Y >= this.Top
            && p.X <= this.Right
            && p.Y <= this.Bottom;
        }

        public IntersectionType Intersects(Vector2L p)
        {
            if (p.X > this.Left && p.Y > this.Top && p.X < this.Right && p.Y < this.Bottom)
            {
                return IntersectionType.Contained;
            }

            if (p.X >= this.Left && p.Y >= this.Top && p.X <= this.Right && p.Y <= this.Bottom)
            {
                return IntersectionType.Intersected;
            }

            return IntersectionType.None;
        }

        public IntersectionType Intersects(RectL p)
        {
            if (p.Left > this.Left && p.Top > this.Top && p.Right < this.Right && p.Bottom < this.Bottom)
            {
                return IntersectionType.Contained;
            }

            if (p.Left >= this.Left && p.Top >= this.Top && p.Right <= this.Right && p.Bottom <= this.Bottom)
            {
                return IntersectionType.Intersected;
            }

            return IntersectionType.None;
        }

        public RectL Encompass(RectL other)
        {
            Vector2L upperLeft = this.LeftTop.Min(other.LeftTop);
            Vector2L size = this.RightBottom.Max(other.RightBottom) - upperLeft;
            return new RectL(upperLeft, size);
        }

        public override bool Equals(object other)
        {
            var typed = (RectL)other;

            return typed.Top.Equals(this.Top) && typed.Left.Equals(this.Left)
                && typed.Bottom.Equals(this.Bottom) && typed.Right.Equals(this.Right);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Top, this.Left, this.Bottom, this.Right).GetHashCode();
        }

        public RectS ToRectS()
        {
            return new RectS((short)this.Left, (short)this.Top, (short)this.Width, (short)this.Height);
        }
    }
}
