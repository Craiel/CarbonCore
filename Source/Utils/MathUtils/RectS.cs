namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("RectS<{Left},{Top} - {Right},{Bottom}>")]
    public struct RectS
    {
        public static RectS Zero = new RectS(0, 0, 0, 0);

        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public RectS(short x, short y, short w, short h)
            : this()
        {
            this.Top = y;
            this.Left = x;
            this.Bottom = (short)(y + h);
            this.Right = (short)(x + w);
        }

        public RectS(RectS that)
            : this(that.LeftTop, that.RightBottom)
        {
        }

        public RectS(Vector2S pos, Vector2S size)
            : this(pos.X, pos.Y, size.X, size.Y)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // -------------------------------------------------------------------
        public short Top { get; set; }
        public short Left { get; set; }
        public short Bottom { get; set; }
        public short Right { get; set; }

        [JsonIgnore]
        public Vector2S LeftTop
        {
            get
            {
                return new Vector2S(this.Top, this.Left);
            }
        }

        [JsonIgnore]
        public Vector2S RightBottom
        {
            get
            {
                return new Vector2S(this.Bottom, this.Right);
            }
        }

        [JsonIgnore]
        public short Width
        {
            get
            {
                return (short)Math.Abs(Math.Abs(this.Left) - Math.Abs(this.Right));
            }
        }

        [JsonIgnore]
        public short Height
        {
            get
            {
                return (short)Math.Abs(Math.Abs(this.Top) - Math.Abs(this.Bottom));
            }
        }

        [JsonIgnore]
        public Vector2S Size
        {
            get
            {
                return new Vector2S(this.Width, this.Height);
            }
        }

        [JsonIgnore]
        public Vector2S Center
        {
            get
            {
                return new Vector2S((short)(this.Left + (this.Right / 2)), (short)(this.Top + (this.Bottom / 2)));
            }
        }

        public bool IsZero
        {
            get
            {
                return this.Equals(Zero);
            }
        }

        public bool Contains(Vector2S p)
        {
            return p.X >= this.Left
            && p.Y >= this.Top
            && p.X <= this.Right
            && p.Y <= this.Bottom;
        }

        public RectS Encompass(RectS other)
        {
            Vector2S upperLeft = this.LeftTop.Min(other.LeftTop);
            Vector2S size = this.RightBottom.Max(other.RightBottom) - upperLeft;
            return new RectS(upperLeft, size);
        }

        public IntersectionType Intersects(Vector2S p)
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

        public IntersectionType Intersects(RectS p)
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

        public override bool Equals(object other)
        {
            var typed = (RectS)other;

            return typed.Top.Equals(this.Top) && typed.Left.Equals(this.Left)
                && typed.Bottom.Equals(this.Bottom) && typed.Right.Equals(this.Right);
        }

        public override int GetHashCode()
        {
            return HashUtils.GetSimpleCombinedHashCode(this.Top, this.Left, this.Bottom, this.Right);
        }
    }
}