namespace CarbonCore.Utils.Unity.Contracts
{
    public interface IUnitySingletonBehavior
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
