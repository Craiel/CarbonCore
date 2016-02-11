namespace CarbonCore.Utils.Unity.Contracts
{
    public interface IUnitySingleton
    {
        bool IsInitialized { get; }
        void Initialize();

        void DestroySingleton();
    }
}
