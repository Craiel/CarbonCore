namespace CarbonCore.Utils.Unity
{
    using System;
    using System.ComponentModel;
    using System.Globalization;

    public static class EnumExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // based on: http://wiki.unity3d.com/index.php?title=EnumExtensions
        public static bool TryParse<T>(string source, out T value)
            where T : struct, IConvertible
        {
            value = default(T);
            if (Enum.IsDefined(typeof(T), source))
            {
                TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
                object fromString = converter.ConvertFromString(source);
                if (fromString != null)
                {
                    value = (T)fromString;
                }

                return true;
            }

            // Alternative method, we try to do a string comparison
            foreach (T entry in Enum.GetValues(typeof(T)))
            {
                if (entry.ToString(CultureInfo.InvariantCulture).Equals(source, StringComparison.OrdinalIgnoreCase))
                {
                    value = entry;
                    return true;
                }
            }

            return false;
        }
    }
}
