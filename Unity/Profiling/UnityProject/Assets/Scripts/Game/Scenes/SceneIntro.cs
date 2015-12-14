namespace Assets.Scripts.Game.Scenes
{
    using Assets.Scripts.Enums;
    using Assets.Scripts.Systems;

    public class SceneIntro : GameScene
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneIntro()
            : base("Intro", "Intro")
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override GameSceneType Type
        {
            get
            {
                return GameSceneType.Intro;
            }
        }
    }
}
