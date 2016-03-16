namespace CarbonCore.Unity.Utils.Contracts
{
    public interface IUnitySingletonBehavior
    {
        bool IsInitialized { get; }

        void Initialize();
    }
}
