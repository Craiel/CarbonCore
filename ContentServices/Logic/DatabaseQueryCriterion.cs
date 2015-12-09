namespace CarbonCore.ContentServices.Compat.Logic
{
    using System;
    using System.Data;

    using CarbonCore.Utils;

    public enum DatabaseQueryCriterionType
    {
        Equals,
        Contains
    }

    public class DatabaseQueryCriterion
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseQueryCriterion(string name, Type internalType, SqlDbType databaseType, params object[] values)
        {
            System.Diagnostics.Trace.Assert(!string.IsNullOrEmpty(name));
            System.Diagnostics.Trace.Assert(values != null && values.Length > 0);

            this.Name = name;
            this.InternalType = internalType;
            this.DatabaseType = databaseType;
            this.Values = values;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; private set; }

        public Type InternalType { get; private set; }

        public SqlDbType DatabaseType { get; private set; }

        public object[] Values { get; private set; }

        public DatabaseQueryCriterionType Type { get; set; }

        public bool Negate { get; set; }

        public override int GetHashCode()
        {
            return HashUtils.CombineObjectHashes(new[] { (object)this.Type, HashUtils.CombineObjectHashes(this.Values), this.Negate });
        }
    }
}
