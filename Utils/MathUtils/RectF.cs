namespace CarbonCore.Utils.MathUtils
{
    using System;
    using System.Diagnostics;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DebuggerDisplay("RectF<{Left},{Top} - {Right},{Bottom}>")]
    public struct RectF
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public RectF(float x, float y, float w, float h)
            : this()
        {
            this.Top = y;
            this.Left = x;
            this.Bottom = y + h;
            this.Right = x + w;
        }

        public RectF(RectF that)
            : this(that.LeftTop, that.RightBottom)
        {
        }

        public RectF(Vector2F pos, Vector2F size)
            : this(pos.X, pos.Y, size.X, size.Y)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // -------------------------------------------------------------------
        public float Top { get; set; }
        public float Left { get; set; }
        public float Bottom { get; set; }
        public float Right { get; set; }

        [JsonIgnore]
        public Vector2F LeftTop
        {
            get
            {
                return new Vector2F(this.Top, this.Left);
            }
        }

        [JsonIgnore]
        public Vector2F RightBottom
        {
            get
            {
                return new Vector2F(this.Bottom, this.Right);
            }
        }

        [JsonIgnore]
        public float Width
        {
            get
            {
                return Math.Abs(Math.Abs(this.Left) - Math.Abs(this.Right));
            }
        }

        [JsonIgnore]
        public float Height
        {
            get
            {
                return Math.Abs(Math.Abs(this.Top) - Math.Abs(this.Bottom));
            }
        }

        [JsonIgnore]
        public Vector2F Size
        {
            get
            {
                return new Vector2F(this.Width, this.Height);
            }
        }
        
        [JsonIgnore]
        public Vector2F Center
        {
            get
            {
                return new Vector2F(this.Left + (this.Right / 2), this.Top + (this.Bottom / 2));
            }
        }
        
        public bool Contains(Vector2F p)
        {
            return p.X >= this.Left
            && p.Y >= this.Top
            && p.X <= this.Right
            && p.Y <= this.Bottom;
        }
        
        public IntersectionType Intersects(Vector2F p)
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

        public IntersectionType Intersects(RectF p)
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
            var typed = (RectF)other;

            return typed.Top.Equals(this.Top) && typed.Left.Equals(this.Left)
                && typed.Bottom.Equals(this.Bottom) && typed.Right.Equals(this.Right);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Top, this.Left, this.Bottom, this.Right).GetHashCode();
        }
    }
}
