namespace CarbonCore.UtilsSDL.Math
{
    using System;

    public abstract class Rect<T, TNVec>
        where T : struct, IComparable
        where TNVec : struct
    {
        // ------------------------------------------------------------------- 
        // Constructor
        // ------------------------------------------------------------------- 
        protected Rect()
            : this(new TNVec(), new TNVec())
        {
        }

        protected Rect(TNVec pos, TNVec size)
        {
            this.Position = pos;
            this.Size = size;
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public abstract T Left { get; }
        public abstract T Right { get; }
        public abstract T Top { get; }
        public abstract T Bottom { get; }

        public TNVec Center { get; protected set; }

        public TNVec Position { get; set; }

        public TNVec Size { get; protected set; }

        public override bool Equals(object other)
        {
            var typed = other as Rect<T, TNVec>;
            if (typed == null)
            {
                return false;
            }

            return typed.Position.Equals(this.Position) && typed.Size.Equals(this.Size);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Position, this.Size).GetHashCode();
        }
    }
}