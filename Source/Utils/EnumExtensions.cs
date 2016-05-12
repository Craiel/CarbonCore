namespace CarbonCore.Utils
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public static class EnumExtensions
    {
        private static readonly IDictionary<Type, object[]> CachedEnumValues = new Dictionary<Type, object[]>();
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static List<T> GetValues<T>(this Enum enumObject)
        {
            return Enum.GetValues(enumObject.GetType()).Cast<T>().ToList();
        }

        public static ReadOnlyCollection<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList().AsReadOnly();
        }

        // Note: preferably it the EnumValues.tt method should be used since it's faster
        //       this is just a handy shortcut
        public static List<T> GetValuesCached<T>(this Enum enumObject)
        {
            return GetValuesCached(enumObject).Cast<T>().ToList();
        }

        public static object[] GetValuesCached(this Enum enumObject)
        {
            return GetValuesCached(enumObject.GetType());
        }

        public static List<T> GetValuesCached<T>()
        {
            return GetValuesCached(typeof(T)).Cast<T>().ToList();
        }

        public static object[] GetValuesCached(Type enumType)
        {
            object[] result;
            if (CachedEnumValues.TryGetValue(enumType, out result))
            {
                return result;
            }

            result = Enum.GetValues(enumType).Cast<object>().ToArray();
            CachedEnumValues.Add(enumType, result);

            return result;
        }
    }
}
