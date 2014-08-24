namespace CarbonCore.Utils
{
    using System;
    using System.Data;

    using CarbonCore.Utils.Collections;
    using CarbonCore.Utils.Contracts;

    public static class DatabaseUtils
    {
        private static readonly IExtendedDictionary<SqlDbType, Type> TypeToDbType = new ExtendedDictionary<SqlDbType, Type> { EnableReverseLookup = true };

        static DatabaseUtils()
        {
            TypeToDbType.Add(SqlDbType.VarChar, typeof(string));
            TypeToDbType.Add(SqlDbType.DateTime, typeof(DateTime));
            TypeToDbType.Add(SqlDbType.Int, typeof(int));
            TypeToDbType.Add(SqlDbType.BigInt, typeof(long));
            TypeToDbType.Add(SqlDbType.Float, typeof(float));
            TypeToDbType.Add(SqlDbType.VarBinary, typeof(byte[]));
            TypeToDbType.Add(SqlDbType.Bit, typeof(bool));
        }

        public static Type GetInternalType(SqlDbType type)
        {
            Type internalType;
            if (TypeToDbType.TryGetValue(type, out internalType))
            {
                return internalType;
            }

            throw new DataException(string.Format("Internal type for type {0} is not implemented", type));
        }

        public static SqlDbType GetDatabaseType(Type internalType)
        {
            if (internalType.IsGenericType && internalType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                internalType = internalType.GetGenericArguments()[0];
            }

            SqlDbType type;
            if (TypeToDbType.TryGetKey(internalType, out type))
            {
                return type;
            }

            throw new DataException(string.Format("Type for internal type {0} is not implemented", internalType));
        }

        public static object TranslateValue(Type target, object value)
        {
            return value;
        }

        public static object TranslateValue(SqlDbType target, object value)
        {
            return value;
        }
    }
}
