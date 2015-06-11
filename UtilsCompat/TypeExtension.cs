namespace CarbonCore.Utils.Compat
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;

    public static class TypeExtension
    {
        public static bool Implements<T>(this Type type) where T : class
        {
            if (!typeof(T).IsInterface)
            {
                throw new InvalidOperationException("Interface type expected for Implements call");
            }

            return typeof(T).IsAssignableFrom(type);
        }

        public static bool ImplementsGenericInterface(this Type type, Type interfaceType)
        {
            return type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType);
        }

        public static T GetCustomAttribute<T>(this Type type, bool inherit = false)
            where T : Attribute
        {
            object[] results = type.GetCustomAttributes(type, inherit);
            if (results.Length <= 0)
            {
                return null;
            }
            
            if (results.Length > 1)
            {
                throw new InvalidDataException("Expected only one attribute but found " + results.Length);
            }

            return results[0] as T;
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsClass || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static object GetDefault(this Type type)
        {
            if (type.IsValueType)
            {
                return Activator.CreateInstance(type);
            }

            return null;
        }

        public static Type GetActualType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return type.GetGenericArguments()[0];
            }

            return type;
        }

        public static object ConvertValue(this Type type, object source)
        {
            bool isNullable = type.IsClass;
            Type targetType = type;
            if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                isNullable = true;
                targetType = targetType.GetGenericArguments()[0];
            }

            if (source == null || source is DBNull)
            {
                if (isNullable)
                {
                    return default(Type);
                }
                
                throw new InvalidDataException("Target type is not nullable, source value is invalid");
            }

            if (source.GetType() == type)
            {
                return source;
            }

            if (targetType == typeof(int))
            {
                return Convert.ToInt32(source);
            }

            if (targetType == typeof(uint))
            {
                return Convert.ToUInt32(source);
            }

            if (targetType == typeof(long))
            {
                return Convert.ToInt64(source);
            }

            if (targetType == typeof(bool))
            {
                return Convert.ToBoolean(source);
            }

            if (targetType == typeof(float))
            {
                return Convert.ToSingle(source);
            }

            if (targetType == typeof(DateTime))
            {
                return Convert.ToDateTime(source);
            }

            if (targetType.IsEnum)
            {
                string name;
                if (source is string)
                {
                    name = source as string;
                }
                else
                {
                    name = Enum.GetName(targetType, source);
                }

                // In case of null we return default
                if (name == null)
                {
                    return Activator.CreateInstance(targetType);
                }

                return Enum.Parse(targetType, name);
            }

            throw new NotImplementedException(string.Format("Can not get Typed value of {0} for target type {1}", source.GetType(), targetType));
        }
    }
}
