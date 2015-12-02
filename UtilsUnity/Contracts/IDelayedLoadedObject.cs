namespace CarbonCore.Utils.Unity.Contracts
{
    public interface IDelayedLoadedObject
    {
        bool IsLoaded { get; }

        bool ContinueLoad();
    }
}
