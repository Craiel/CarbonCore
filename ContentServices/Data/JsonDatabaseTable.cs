﻿namespace CarbonCore.ContentServices.Data
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    [DataEntry(UseDefaultEquality = true)]
    public class JsonDatabaseTable : DataEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JsonDatabaseTable()
        {
            this.RowData = new Dictionary<int, string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [DatabaseEntryElement]
        public int NextPrimaryKey { get; set; }

        [DatabaseEntryElement]
        public Dictionary<int, string> RowData { get; set; }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override int DoGetHashCode()
        {
            throw new InvalidOperationException();
        }
    }
}