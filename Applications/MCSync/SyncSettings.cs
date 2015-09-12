namespace CarbonCore.Applications.MCSync
{
    using CarbonCore.ContentServices.Compat.Logic.Attributes;
    using CarbonCore.ContentServices.Compat.Logic.DataEntryLogic;

    [DataEntry]
    public class SyncSettings : DataEntry
    {
        public SyncSettings()
        {
            this.Version = 1;
        }

        [DataElement]
        public int Version { get; set; }
    }
}
