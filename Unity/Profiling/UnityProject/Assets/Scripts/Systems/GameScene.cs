namespace Assets.Scripts.Systems
{
    using Assets.Scripts.Enums;
    using Assets.Scripts.Systems.Contracts;
    
    using CarbonCore.Utils.Unity.Logic.Scene;

    public abstract class GameScene : BaseScene, IGameScene
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected GameScene(string name, string levelName)
            : base(name, levelName)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract GameSceneType Type { get; }
        
        protected override bool ScenePostDestroy()
        {
            // This should always be last, it's cleaning up the instance objects
            SceneController.Instance.Clear();

            return base.ScenePostDestroy();
        }
    }
}
