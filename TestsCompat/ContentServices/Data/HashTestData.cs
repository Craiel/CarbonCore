namespace CarbonCore.Tests.Compat.ContentServices.Data
{
    using CarbonCore.ContentServices.Compat.Logic.Attributes;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;

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
