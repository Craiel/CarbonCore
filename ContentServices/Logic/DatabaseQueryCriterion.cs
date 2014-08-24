namespace CarbonCore.ContentServices.Logic
{
    using System;

    using CarbonCore.ContentServices.Logic.Attributes;
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
        public DatabaseQueryCriterion(AttributedPropertyInfo<DatabaseEntryElementAttribute> attribute, params object[] values)
        {
            System.Diagnostics.Trace.Assert(attribute != null);
            System.Diagnostics.Trace.Assert(values != null && values.Length > 0);

            this.Attribute = attribute;
            this.Values = values;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public AttributedPropertyInfo<DatabaseEntryElementAttribute> Attribute { get; private set; }

        public object[] Values { get; private set; }

        public DatabaseQueryCriterionType Type { get; set; }

        public bool Negate { get; set; }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Type, HashUtils.CombineObjectHashes(this.Values), this.Negate).GetHashCode();
        }
    }
}
