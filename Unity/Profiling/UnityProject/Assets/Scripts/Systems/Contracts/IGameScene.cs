namespace Assets.Scripts.Systems.Contracts
{
    using Assets.Scripts.Enums;

    using CarbonCore.Utils.Unity.Contracts;

    public interface IGameScene : IScene
    {
        GameSceneType Type { get; }
    }
}
