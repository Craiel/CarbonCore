namespace CarbonCore.Tests.ContentServices.Data
{
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    public class HashTestData : DataEntry
    {
        [DataElement]
        public int Id { get; set; }

        [DataElement]
        public bool Bool { get; set; }

        protected override int DoGetHashCode()
        {
            return this.Id.GetHashCode() ^ this.Bool.GetHashCode();
        }
    }
}
