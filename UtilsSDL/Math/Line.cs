namespace CarbonCore.UtilsSDL.Math
{
    using System;

    public abstract class Line<T, TNVec>
        where T : struct, IComparable
        where TNVec : struct
    {
        // ------------------------------------------------------------------- 
        // Constructor
        // ------------------------------------------------------------------- 
        protected Line()
        {
        }

        protected Line(TNVec start, TNVec end)
        {
            this.Start = start;
            this.End = end;
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public TNVec Start { get; set; }
        public TNVec End { get; set; }

        public override bool Equals(object other)
        {
            var typed = other as Line<T, TNVec>;
            if (typed == null)
            {
                return false;
            }

            return typed.Start.Equals(this.Start) && typed.End.Equals(this.End);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Start, this.End).GetHashCode();
        }
    }
}