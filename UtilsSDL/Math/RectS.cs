namespace CarbonCore.UtilsSDL.Math
{
    public class RectS : Rect<short, Vector2S>
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public RectS()
            : this(0, 0, 0, 0)
        {
        }

        public RectS(short x, short y, short w, short h)
            : this(new Vector2S(x, y), new Vector2S(w, h))
        {
        }

        public RectS(RectS that)
            : this(that.Position, that.Size)
        {
        }

        public RectS(Vector2S pos, Vector2S size)
            : base(pos, size)
        {
            this.Center = new Vector2S((short)(pos.X + (size.X / 2)), (short)(pos.Y + (size.Y / 2)));
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override short Left
        {
            get
            {
                return this.Position.X;
            }
        }

        public override short Right
        {
            get
            {
                return (short)(this.Position.X + this.Size.X);
            }
        }

        public override short Top
        {
            get
            {
                return this.Position.Y;
            }
        }

        public override short Bottom
        {
            get
            {
                return (short)(this.Position.Y + this.Size.Y);
            }
        }

        public short Width
        {
            get
            {
                return this.Size.X;
            }
        }

        public short Height
        {
            get
            {
                return this.Size.Y;
            }
        }

        public bool HasContent
        {
            get
            {
                return this.Size.X > 0 && this.Size.Y > 0;
            }
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
            
            if (p.Left <= this.Right && p.Top <= this.Bottom && p.Right >= this.Left && p.Bottom >= this.Top)
            {
                return IntersectionType.Intersected;
            }
            
            return IntersectionType.None;
        }

        public void Encompass(RectS rect)
        {
            Vector2S upperLeft = this.Position.Min(rect.Position);
            Vector2S lowerRight = (this.Position + this.Size).Max(rect.Position + rect.Size);
            this.Position = upperLeft;
            this.Size = new Vector2S((short)(lowerRight.X - upperLeft.X), (short)(lowerRight.Y - upperLeft.Y));
        }
    }
}
