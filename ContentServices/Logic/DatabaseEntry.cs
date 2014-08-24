namespace CarbonCore.ContentServices.Logic
{
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.Attributes;
    using CarbonCore.Utils;

    public class DatabaseEntry : ContentEntry, IDatabaseEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DatabaseEntry()
        {
            this.Descriptor = DatabaseEntryDescriptor.GetDescriptor(this.GetType());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<string> GetElementNames()
        {
            return this.Descriptor.GetElementNames();
        }

        public IList<object> GetValues()
        {
            IList<object> result = new List<object>();
            foreach (AttributedPropertyInfo<DatabaseEntryElementAttribute> info in this.Descriptor.PropertyInfos)
            {
                result.Add(info.Property.GetValue(this));
            }

            return result;
        }

        public void SetValues(IDictionary<string, object> values)
        {
            foreach (string name in values.Keys)
            {
                AttributedPropertyInfo<DatabaseEntryElementAttribute> element = this.Descriptor.GetElementByName(name);
                element.Property.SetValue(this, values[name]);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected DatabaseEntryDescriptor Descriptor { get; private set; }
    }
}
