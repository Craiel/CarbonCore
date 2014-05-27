namespace CarbonCore.UtilsSDL.Math
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    public class RectL : Rect<long, Vector2L>
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        // Zero
        public RectL()
            : this(0, 0, 0, 0)
        {
        }

        // Uniform Rect
        public RectL(Vector2L p, long s)
            : this(p.X, p.Y, s, s)
        {
        }

        public RectL(long x, long y, long w, long h)
            : this(new Vector2L(x, y), new Vector2L(w, h))
        {
        }

        public RectL(RectL that)
            : this(that.Position, that.Size)
        {
        }

        public RectL(Vector2L pos, Vector2L size)
            : base(pos, size)
        {
            this.Center = new Vector2L(pos.X + (size.X / 2), pos.Y + (size.Y / 2));
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override long Left
        {
            get
            {
                return this.Position.X;
            }
        }

        public override long Right
        {
            get
            {
                return this.Position.X + this.Size.X;
            }
        }

        public override long Top
        {
            get
            {
                return this.Position.Y;
            }
        }

        public override long Bottom
        {
            get
            {
                return this.Position.Y + this.Size.Y;
            }
        }
        
        public IList<Vector2L> GenerateCircleRectOffsets(ushort distance, bool fill = true)
        {
            if (distance <= 0)
            {
                throw new ArgumentException("GenerateRectAround called with distance 0");
            }

            IList<Vector2L> results = new List<Vector2L>();
            
            // Project a cone in all directions
            this.GenerateConeRectOffsets(results, Direction.Up, distance, fill);
            this.GenerateConeRectOffsets(results, Direction.Down, distance, fill);
            this.GenerateConeRectOffsets(results, Direction.Left, distance, fill, skipSides: true);
            this.GenerateConeRectOffsets(results, Direction.Right, distance, fill, skipSides: true);

            return results;
        }

        public int GenerateConeRectOffsets(IList<Vector2L> list, Direction direction, ushort distance, bool fill = true, bool skipSides = false)
        {
            if (distance <= 0)
            {
                throw new ArgumentException("GenerateRectAround called with distance 0");
            }

            var offset = new Vector2L(this.Position);
            int count = 0;
            for (int dY = 1; dY <= distance; dY++)
            {
                // Center pieces
                switch (direction)
                {
                    case Direction.Up:
                        {
                            offset.X = this.Position.X;
                            offset.Y -= this.Size.Y;
                            break;
                        }

                    case Direction.Down:
                        {
                            offset.X = this.Position.X;
                            offset.Y += this.Size.Y;
                            break;
                        }

                    case Direction.Left:
                        {
                            offset.Y = this.Position.Y;
                            offset.X -= this.Size.X;
                            break;
                        }

                    case Direction.Right:
                        {
                            offset.Y = this.Position.Y;
                            offset.X += this.Size.X;
                            break;
                        }

                    default:
                        {
                            throw new NotImplementedException();
                        }
                }

                if (fill || dY == distance)
                {
                    list.Add(new Vector2L(offset));
                    count++;
                }

                // Sides
                int maxSides = 1 * dY;
                for (int dX = 1; dX <= maxSides; dX++)
                {
                    if (!fill && dX < maxSides)
                    {
                        continue;
                    }

                    if (skipSides && dX == maxSides)
                    {
                        continue;
                    }

                    switch (direction)
                    {
                        case Direction.Down:
                        case Direction.Up:
                            {
                                offset.X = this.Position.X - (dX * this.Size.X);
                                list.Add(new Vector2L(offset));

                                offset.X = this.Position.X + (dX * this.Size.X);
                                list.Add(new Vector2L(offset));

                                break;
                            }

                        case Direction.Left:
                        case Direction.Right:
                            {
                                offset.Y = this.Position.Y - (dX * this.Size.Y);
                                list.Add(new Vector2L(offset));

                                offset.Y = this.Position.Y + (dX * this.Size.Y);
                                list.Add(new Vector2L(offset));
                                break;
                            }

                        default:
                            {
                                throw new NotImplementedException();
                            }
                    }
                    
                    count += 2;
                }
            }

            return count;
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
        
        public void Encompass(RectL rect)
        {
            Vector2L upperLeft = this.Position.Min(rect.Position);
            Vector2L lowerRight = (this.Position + this.Size).Max(rect.Position + rect.Size);
            this.Position = upperLeft;
            this.Size = new Vector2L(lowerRight.X - upperLeft.X, lowerRight.Y - upperLeft.Y);
        }

        public RectS ToRectS()
        {
            System.Diagnostics.Debug.Assert(Math.Abs(this.Position.X) <= short.MaxValue, "Position exceeds short range");
            System.Diagnostics.Debug.Assert(Math.Abs(this.Position.Y) <= short.MaxValue, "Position exceeds short range");
            System.Diagnostics.Debug.Assert(Math.Abs(this.Size.X) <= short.MaxValue, "Size exceeds short range");
            System.Diagnostics.Debug.Assert(Math.Abs(this.Size.X) <= short.MaxValue, "Size exceeds short range");

            return new RectS((short)this.Position.X, (short)this.Position.Y, (short)this.Size.X, (short)this.Size.Y);
        }
    }
}
