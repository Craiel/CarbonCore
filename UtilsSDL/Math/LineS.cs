namespace CarbonCore.UtilsSDL.Math
{
    public class LineS : Line<short, Vector2S>
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public LineS(short x1, short y1, short x2, short y2)
            : base(new Vector2S(x1, y1), new Vector2S(x2, y2))
        {
        }

        public LineS(Vector2S start, Vector2S end)
            : base(start, end)
        {
        }

        public LineS(LineS that)
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

        public RectS GetBounds()
        {
            var upperLeft = new Vector2S(0, 0);
            var lowerRight = new Vector2S(0, 0);
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

            return new RectS(upperLeft, new Vector2S((short)(lowerRight.X - upperLeft.X), (short)(lowerRight.Y - upperLeft.Y)));
        }
    }
}
