namespace CarbonCore.UtilsSDL.Math
{
    public class LineL : Line<long, Vector2L>
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public LineL(long x1, long y1, long x2, long y2)
            : base(new Vector2L(x1, y1), new Vector2L(x2, y2))
        {
        }

        public LineL(Vector2L start, Vector2L end)
            : base(start, end)
        {
        }

        public LineL(LineL that)
            : base(that.Start, that.End)
        {
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public bool IsVertical
        {
            get
            {
                return this.End.X - this.Start.X == 0;
            }
        }

        public bool IsHorizontal
        {
            get
            {
                return this.End.Y - this.Start.Y == 0;
            }
        }

        public RectL GetBounds()
        {
            var upperLeft = new Vector2L(0, 0);
            var lowerRight = new Vector2L(0, 0);
            if (this.IsHorizontal)
            {
                upperLeft.X = this.Start.X;
                lowerRight.X = this.Start.X;
            }
            else
            {
                if (this.Start.X < this.End.X)
                {
                    upperLeft.X = this.Start.X;
                    lowerRight.X = this.End.X;
                }
                else
                {
                    upperLeft.X = this.End.X;
                    lowerRight.X = this.Start.X;
                }
            }

            if (this.IsVertical)
            {
                upperLeft.Y = this.Start.Y;
                lowerRight.Y = this.Start.Y;
            }
            else
            {
                if (this.Start.Y < this.End.Y)
                {
                    upperLeft.Y = this.Start.Y;
                    lowerRight.Y = this.End.Y;
                }
                else
                {
                    upperLeft.Y = this.End.Y;
                    lowerRight.Y = this.Start.Y;
                }
            }

            return new RectL(upperLeft, new Vector2L(lowerRight.X - upperLeft.X, lowerRight.Y - upperLeft.Y));
        }
    }
}
