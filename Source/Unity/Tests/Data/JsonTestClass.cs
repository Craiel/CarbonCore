namespace CarbonCore.Unity.Tests.Data
{
    using System;
    using System.Collections.Generic;
    
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [DataEntry(UseDefaultEquality = true)]
    public class JsonTestClass : SmartDataEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DataElement, JsonConverter(typeof(JsonDictionaryConverter<JsonTestSubClass, int>))]
        public Dictionary<JsonTestSubClass, int> KeyDictionary { get; set; }

        [DataElement, JsonConverter(typeof(JsonDictionaryConverter<string, JsonTestSubClass>))]
        public Dictionary<string, JsonTestSubClass> ValueDictionary { get; set; }

        [DataElement, JsonConverter(typeof(JsonDictionaryConverter<JsonTestSubClass, JsonTestSubClass>))]
        public Dictionary<JsonTestSubClass, JsonTestSubClass> KeyValueDictionary { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            throw new InvalidOperationException();
        }
    }
}