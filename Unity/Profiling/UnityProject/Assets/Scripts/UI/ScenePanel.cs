namespace Assets.Scripts.UI
{
    using Assets.Scripts.Enums;

    public abstract class ScenePanel : BasePanel
    {
        public abstract GameSceneType Type { get; }
    }
}
