namespace CarbonCore.Utils.MathUtils
{
    using System;

    public static class FloatExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool NullableEquals(this float? a, float? b, float precision)
        {
            if (a == null)
            {
                return b == null;
            }

            if (b == null)
            {
                return false;
            }

            return Math.Abs(a.Value - b.Value) < precision;
        }

        public static bool NullableEquals(this float? a, float? b)
        {
            return a.NullableEquals(b, float.Epsilon);
        }
    }
}
