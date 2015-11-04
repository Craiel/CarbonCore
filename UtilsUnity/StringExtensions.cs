namespace CarbonCore.Utils.Unity
{
    using System;

    public static class StringExtensions
    {
        public static bool TryParseEnum<T>(this string strEnumValue, out T? value)
            where T : struct, IConvertible
        {
            value = null;
            if (!Enum.IsDefined(typeof(T), strEnumValue))
            {
                return false;
            }

            value = (T)Enum.Parse(typeof(T), strEnumValue);
            return true;
        }
    }
}
