namespace CarbonCore.Utils.Compat.Database
{
    using System;
    using System.Data;
    using System.IO;

    using CarbonCore.Utils.Compat.Collections;
    using CarbonCore.Utils.Compat.Contracts;

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
            Type actualType = internalType.GetActualType();
            SqlDbType type;
            if (TypeToDbType.TryGetKey(actualType, out type))
            {
                return type;
            }

            throw new DataException(string.Format("Type for internal type {0} is not implemented", internalType));
        }

        public static string GetDatabaseTypeString(Type internalType)
        {
            SqlDbType databaseType = GetDatabaseType(internalType);
            return GetDatabaseTypeString(databaseType);
        }

        public static string GetDatabaseTypeString(SqlDbType type)
        {
            switch (type)
            {
                case SqlDbType.Int:
                    return "INTEGER";

                default:
                    {
                        return type.ToString().ToUpperInvariant();
                    }
            }
        }

        public static object GetInternalValue(SqlDbType databaseType, object source)
        {
            Type internalType;
            if (!TypeToDbType.TryGetValue(databaseType, out internalType))
            {
                throw new InvalidDataException("Could not map db type for " + databaseType);
            }

            return internalType.ConvertValue(source);
        }
    }
}