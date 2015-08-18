namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System;

    using CarbonCore.ContentServices.Contracts;

    public abstract class SyncEntry : DataEntry, ISyncEntry
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected SyncEntry()
        {
            DataEntrySyncDescriptor descriptor = DataEntryDescriptors.GetSyncDescriptor(this.GetType());
            this.Content = new ISyncContent[descriptor.Entries.Count];
            for (var i = 0; i < descriptor.Entries.Count; i++)
            {
                var instance = (ISyncContent)Activator.CreateInstance(descriptor.Entries[i].Type);
                this.Content[i] = instance;
                descriptor.Entries[i].Property.SetValue(this, instance);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ISyncContent[] Content { get; private set; }

        public void ResetSyncState()
        {
            foreach (ISyncContent content in this.Content)
            {
                content.ResetChangeState();
            }
        }
    }
}
