namespace CarbonCore.Unity.Utils.Contracts
{
    public interface IDelayedLoadedObject
    {
        bool IsLoading { get; }

        bool IsLoaded { get; }

        bool ContinueLoad();
    }
}
