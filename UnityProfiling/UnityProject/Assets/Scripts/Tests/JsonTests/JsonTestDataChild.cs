namespace Assets.Scripts.Tests.JsonTests
{
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    public class JsonTestDataChild : DataEntry
    {
        private static int NextId;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JsonTestDataChild()
        {
            this.Id = ++NextId;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int Id { get; set; }

        public string ParentString { get; set; }

        protected override int DoGetHashCode()
        {
            return this.Id;
        }
    }
}
