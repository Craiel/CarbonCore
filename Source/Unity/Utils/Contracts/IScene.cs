namespace CarbonCore.Unity.Utils.Contracts
{
    using CarbonCore.Unity.Utils.Logic.Enums;

    public interface IScene
    {
        string Name { get; }

        bool ContinueLoad(SceneTransitionStep step);

        bool ContinueDestroy(SceneTransitionStep step);

        void SetData(object[] data);
    }
}
