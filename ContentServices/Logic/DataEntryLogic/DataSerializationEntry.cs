namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;

    public class DataSerializationEntry
    {
        public DataSerializationEntry(Type source, AttributedPropertyInfo<DataElementAttribute> property)
        {
            this.Key = new DataSerializationKey(source, property);
        }

        public DataSerializationKey Key { get; private set; }

        public bool IsDataEntry { get; set; }

        public bool IsNullable { get; set; }

        public Type Target { get; set; }
    }
}
