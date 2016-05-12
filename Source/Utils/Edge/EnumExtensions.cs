namespace CarbonCore.Utils.Edge
{
    using System;
    using System.Globalization;

    public static class EnumExtensions
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static bool TryParseInvariant<T>(string source, out T value)
            where T : struct, IConvertible
        {
            if (Enum.TryParse(source, out value))
            {
                return true;
            }

            // Alternative method, we try to do a string comparison
            foreach (T entry in Utils.EnumExtensions.GetValuesCached<T>())
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
