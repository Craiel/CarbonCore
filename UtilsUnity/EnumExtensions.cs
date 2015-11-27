namespace CarbonCore.Utils.Unity
{
    using System;
    using System.ComponentModel;

    public static class EnumExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // Source: http://wiki.unity3d.com/index.php?title=EnumExtensions
        public static bool TryParse<T>(string source, out T value)
            where T : struct, IConvertible
        {
            value = default(T);
            if (Enum.IsDefined(typeof(T), source))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                value = (T)converter.ConvertFromString(source);
                return true;
            }

            return false;
        }
    }
}
