﻿namespace CarbonCore.ContentServices.Logic
{
    using System.Collections.Generic;

    using CarbonCore.ContentServices.Contracts;
    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    public abstract class DatabaseEntry : DataEntry, IDatabaseEntry
    {
        private readonly DatabaseEntryDescriptor descriptor;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected DatabaseEntry()
        {
            this.descriptor = DatabaseEntryDescriptor.GetDescriptor(this.GetType());
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public IList<string> GetElementNames()
        {
            IList<string> results = new List<string>();
            foreach (DatabaseEntryElementDescriptor element in this.descriptor.Elements)
            {
                results.Add(element.Name);
            }

            return results;
        }

        public IList<object> GetValues()
        {
            IList<object> result = new List<object>();
            foreach (DatabaseEntryElementDescriptor info in this.descriptor.Elements)
            {
                result.Add(info.GetValue(this));
            }

            return result;
        }

        public DatabaseEntryDescriptor GetDescriptor()
        {
            return this.descriptor;
        }
    }
}