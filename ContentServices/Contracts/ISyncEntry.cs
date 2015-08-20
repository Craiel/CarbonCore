namespace CarbonCore.ContentServices.Contracts
{
    using System.IO;

    public interface ISyncEntry
    {
        int Save(Stream target);
        void Load(Stream source);

        void ResetSyncState(bool state = false);
    }
}
