namespace CarbonCore.Utils
{
    using System;

    public static class RandomExtension
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static long NextLong(this Random rand)
        {
            return rand.NextLong(long.MinValue, long.MaxValue);
        }

        public static long NextLong(this Random rand, long min)
        {
            return rand.NextLong(min, long.MaxValue);
        }

        public static long NextLong(this Random rand, long min, long max)
        {
            long result = rand.Next((int)min >> 32, (int)max >> 32);
            result = result << 32;
            result = result | rand.Next((int)min, (int)max);
            return result;
        }
    }
}
