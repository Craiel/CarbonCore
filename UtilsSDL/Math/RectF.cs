namespace CarbonCore.UtilsSDL.Math
{
    using System;

    public class RectF : Rect<float, Vector2F>
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public RectF(float x, float y, float w, float h)
            : this(new Vector2F(x, y), new Vector2F(w, h))
        {
        }

        public RectF(RectF that)
            : this(that.Position, that.Size)
        {
        }

        public RectF(Vector2F pos, Vector2F size)
            : base(pos, size)
        {
            this.Center = new Vector2F(pos.X + (size.X / 2), pos.Y + (size.Y / 2));
        }

        // ------------------------------------------------------------------- 
        // Public 
        // -------------------------------------------------------------------
        public override float Left
        {
            get
            {
                return this.Position.X;
            }
        }

        public override float Right
        {
            get
            {
                return this.Position.X + this.Size.X;
            }
        }

        public override float Top
        {
            get
            {
                return this.Position.Y;
            }
        }

        public override float Bottom
        {
            get
            {
                return this.Position.Y + this.Size.Y;
            }
        }
        
        public bool Contains(Vector2F p)
        {
            return p.X > this.Position.X
            && p.Y > this.Position.Y
            && p.X < this.Position.X + this.Size.X
            && this.Position.Y < this.Position.Y + this.Size.Y;
        }

        public bool Contains(Vector2F p, Vector2F exit)
        {
            throw new NotImplementedException();
        }
        
        public bool Contains(Vector2F position, Vector2F movingDirection, out Vector2F? exit)
        {
            throw new NotImplementedException();

            exit = null;

            // Quick discard
            if (!this.Contains(position))
            {
                return false;
            }

            // We're contained in this rect. Find the exit on the edge
            // Algorithm:
            // - Using the direction and position, detect on which edge are we performing collision
            // - Negate the less significant component of the direction
            // - Use the distance between the dot and the edge to find the point
        }
    }
}
