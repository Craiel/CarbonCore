namespace Assets.Scripts.Game.Scenes
{
    using Assets.Scripts.Data;
    using Assets.Scripts.Enums;
    using Assets.Scripts.Systems;

    using CarbonCore.Utils.Unity.Logic.Resource;

    public class SceneMainMenu : GameScene
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneMainMenu()
            : base("Main Menu", "MainMenu")
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override GameSceneType Type
        {
            get
            {
                return GameSceneType.MainMenu;
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override bool SceneRegisterResources1()
        {
            ResourceProvider.Instance.RegisterResource(AssetResourceKeys.SfxAcceptAssetKey);

            return base.SceneRegisterResources1();
        }
    }
}
