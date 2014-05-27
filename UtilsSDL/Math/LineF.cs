namespace CarbonCore.UtilsSDL.Math
{
    public class LineF : Line<float, Vector2F>
    {
        // ------------------------------------------------------------------- 
        // Constructor 
        // ------------------------------------------------------------------- 
        public LineF(float x1, float y1, float x2, float y2)
            : base(new Vector2F(x1, y1), new Vector2F(x2, y2))
        {
        }

        public LineF(Vector2F start, Vector2F end)
            : base(start, end)
        {
        }

        public LineF(LineF that)
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
                return System.Math.Abs(this.End.X - this.Start.X) < float.Epsilon;
            }
        }

        public bool IsHorizontal
        {
            get
            {
                return System.Math.Abs(this.End.Y - this.Start.Y) < float.Epsilon;
            }
        }
    }
}
