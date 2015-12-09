namespace CarbonCore.Tests.Unity.Data
{
    using CarbonCore.ContentServices.Compat.Logic.Attributes;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;

    [DataEntry(UseDefaultEquality = true)]
    public class BufferTestEntry : DataEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DataElement]
        public int TestInt { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            return this.TestInt.GetHashCode();
        }
    }
}
