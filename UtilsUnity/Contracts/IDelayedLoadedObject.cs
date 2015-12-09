namespace CarbonCore.Utils.Unity.Contracts
{
    public interface IDelayedLoadedObject
    {
        bool IsLoading { get; }

        bool IsLoaded { get; }

        bool ContinueLoad();
    }
}
