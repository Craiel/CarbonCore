namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.IO;

    using CarbonCore.ContentServices.Contracts;

    public abstract class SyncEntry : ISyncEntry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract long Save(Stream target);

        public abstract void Load(Stream source);

        public abstract void ResetSyncState(bool state = false);
    }
}
