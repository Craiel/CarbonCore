namespace CarbonCore.Unity.Utils.Contracts
{
    public interface IUnitySingleton
    {
        bool IsInitialized { get; }
        void Initialize();

        void DestroySingleton();
    }
}
