namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;

    public static class ContentReflection
    {
        private static readonly IDictionary<Type, string> TableNameCache;
        private static readonly IDictionary<Type, IList<ContentReflectionProperty>> PropertyLookupCache;
        private static readonly IDictionary<Type, ContentReflectionProperty> PrimaryKeyPropertyLookupCache; 

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static ContentReflection()
        {
            TableNameCache = new Dictionary<Type, string>();
            PropertyLookupCache = new Dictionary<Type, IList<ContentReflectionProperty>>();
            PrimaryKeyPropertyLookupCache = new Dictionary<Type, ContentReflectionProperty>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static string GetTableName(Type key)
        {
            lock (TableNameCache)
            {
                if (!TableNameCache.ContainsKey(key))
                {
                    var attribute =
                        key.GetCustomAttributes(typeof(ContentEntryAttribute), true).FirstOrDefault() as
                        ContentEntryAttribute;
                    if (attribute == null)
                    {
                        throw new InvalidOperationException("Unknown error finding table specification");
                    }

                    TableNameCache.Add(key, attribute.Table);
                }

                return TableNameCache[key];
            }
        }

        public static string GetTableName<T>() where T : IDatabaseEntry
        {
            return GetTableName(typeof(T));
        }

        public static IList<ContentReflectionProperty> GetPropertyInfos(Type type)
        {
            if (!PropertyLookupCache.ContainsKey(type))
            {
                BuildLookupCache(type);
            }
            
            return PropertyLookupCache[type];
        }

        public static IList<ContentReflectionProperty> GetPropertyInfos<T>() where T : ICarbonContent
        {
            return GetPropertyInfos(typeof(T));
        }

        public static ContentReflectionProperty GetPrimaryKeyPropertyInfo(Type type)
        {
            if (!PrimaryKeyPropertyLookupCache.ContainsKey(type))
            {
                BuildLookupCache(type);
            }

            return PrimaryKeyPropertyLookupCache[type];
        }

        public static ContentReflectionProperty GetPrimaryKeyPropertyInfo<T>() where T : ICarbonContent
        {
            return GetPrimaryKeyPropertyInfo(typeof(T));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void BuildLookupCache(Type type)
        {
            lock (PropertyLookupCache)
            {
                IList<ContentReflectionProperty> properties = new List<ContentReflectionProperty>();
                PropertyInfo[] propertyInfos = type.GetProperties();
                foreach (PropertyInfo info in propertyInfos)
                {
                    IEnumerable<Attribute> attributes = info.GetCustomAttributes();
                    foreach (Attribute attribute in attributes)
                    {
                        Type attributeType = attribute.GetType();
                        if (attributeType == typeof(DatabaseEntryPrimaryKeyAttribute))
                        {
                            continue;
                        }

                        if (attributeType == typeof(DatabaseEntryElementAttribute))
                        {
                            continue;
                        }
                    }

                    var attribute =
                        info.GetCustomAttributes(typeof(ContentEntryElementAttribute), false).FirstOrDefault() as
                        ContentEntryElementAttribute;
                    if (attribute != null)
                    {
                        var propertyInfo = new ContentReflectionProperty(attribute, info)
                                               {
                                                   PrimaryKey =
                                                       attribute.PrimaryKey
                                               };
                        properties.Add(propertyInfo);

                        if (attribute.PrimaryKey != PrimaryKeyMode.None)
                        {
                            if (PrimaryKeyPropertyLookupCache.ContainsKey(type))
                            {
                                throw new DataException("Only one primary key is currently supported for type " + type);
                            }

                            PrimaryKeyPropertyLookupCache.Add(type, propertyInfo);
                        }
                    }
                }

                if (!PrimaryKeyPropertyLookupCache.ContainsKey(type))
                {
                    throw new DataException("Type does not have a primary key defined: " + type);
                }

                PropertyLookupCache.Add(type, properties);
            }
        }
    }
}
