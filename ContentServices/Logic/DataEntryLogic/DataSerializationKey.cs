namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;

    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils.Compat;

    public class DataSerializationKey
    {
        public DataSerializationKey(Type source, AttributedPropertyInfo<DataElementAttribute> property)
        {
            this.Source = source;
            this.Property = property;
        }

        public Type Source { get; private set; }

        public AttributedPropertyInfo<DataElementAttribute> Property { get; private set; }
    }
}
