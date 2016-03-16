namespace CarbonCore.Unity.Utils.Contracts
{
    using CarbonCore.Utils.Threading;

    public interface IEngineComponent
    {
        bool IsInitialized { get; }

        void Initialize();
        void Destroy();

        void Update(EngineTime time);
    }
}
