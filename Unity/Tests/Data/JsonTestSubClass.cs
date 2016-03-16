namespace CarbonCore.Unity.Tests.Data
{
    using System;
    
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    [DataEntry(UseDefaultEquality = true)]
    public class JsonTestSubClass : DataEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string String { get; set; }

        public int Int { get; set; }

        public float Float { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            throw new InvalidOperationException();
        }
    }
}
