namespace CarbonCore.ContentServices.Contracts
{
    public interface ISyncEntry
    {
        ISyncContent[] Content { get; }

        void ResetSyncState();
    }
}
