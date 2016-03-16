namespace CarbonCore.Utils.MathUtils
{
    public static class BitOperations
    {
        public static uint RotateLeft(this uint value, int count)
        {
            return (value << count) | (value >> (32 - count));
        }

        public static uint RotateRight(this uint value, int count)
        {
            return (value >> count) | (value << (32 - count));
        }

        public static uint[] SplitLong(long value)
        {
            var lower = (uint)(value & uint.MaxValue);
            var upper = (uint)(value >> 32);
            return new[] { lower, upper };
        }

        public static long MergeLong(uint lower, uint upper)
        {
            long value = upper;
            value = value << 32;
            value = value | lower;
            return value;
        }
    }
}
