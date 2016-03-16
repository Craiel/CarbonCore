namespace CarbonCore.Unity.Tests.Data
{
    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Unity.Utils.Contracts.BufferedData;

    [DataEntry(UseDefaultEquality = true)]
    public class BufferTestEntry : IBufferedDataEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int TestInt { get; set; }

        public IDataEntry Clone()
        {
            return new BufferTestEntry { TestInt = this.TestInt };
        }
    }
}
