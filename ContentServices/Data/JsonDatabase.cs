namespace CarbonCore.ContentServices.Data
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DataEntry(UseDefaultEquality = true)]
    public class JsonDatabase : DataEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JsonDatabase()
        {
            this.Tables = new Dictionary<string, JsonDatabaseTable>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DatabaseEntryElement, JsonConverter(typeof(JsonDictionaryConverter<string, JsonDatabaseTable>))]
        public Dictionary<string, JsonDatabaseTable> Tables { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            throw new InvalidOperationException();
        }
    }
}
