namespace CarbonCore.Utils.Compat.MathUtils
{
    using System;

    public static class FloatExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool NullableEquals(this float? a, float? b)
        {
            if (a == null)
            {
                return b == null;
            }

            if (b == null)
            {
                return false;
            }

            return Math.Abs(a.Value - b.Value) < float.Epsilon;
        }
    }
}
