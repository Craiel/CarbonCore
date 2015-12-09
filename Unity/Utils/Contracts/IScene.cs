namespace CarbonCore.Utils.Unity.Contracts
{
    using CarbonCore.Utils.Unity.Logic.Enums;

    public interface IScene
    {
        string Name { get; }

        bool ContinueLoad(SceneTransitionStep step);

        bool ContinueDestroy(SceneTransitionStep step);

        void SetData(object[] data);
    }
}
