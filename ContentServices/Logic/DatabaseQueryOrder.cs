namespace CarbonCore.ContentServices.Logic
{
    using System;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;

    public class DatabaseQueryOrder
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseQueryOrder(AttributedPropertyInfo<DatabaseEntryElementAttribute> attribute)
        {
            System.Diagnostics.Trace.Assert(attribute != null);

            this.Attribute = attribute;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public AttributedPropertyInfo<DatabaseEntryElementAttribute> Attribute { get; private set; }

        public bool Ascending { get; set; }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Ascending).GetHashCode();
        }
    }
}
