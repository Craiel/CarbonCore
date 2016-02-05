﻿namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.Data;

    using CarbonCore.Utils;
    using CarbonCore.Utils.Diagnostics;

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
            Diagnostic.Assert(!string.IsNullOrEmpty(name));
            Diagnostic.Assert(values != null && values.Length > 0);

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
