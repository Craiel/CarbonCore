namespace CarbonCore.Utils.Contracts
{
    public interface IThreadQueueComponent
    {
        bool HasQueuedOperations { get; }
    }
}
