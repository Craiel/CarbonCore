namespace Assets.Scripts.Game
{
    using Assets.Scripts.Data;
    using Assets.Scripts.Enums;
    using Assets.Scripts.UI;

    using UnityEngine;

    public class GameInit : MonoBehaviour
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public GameInit()
        {
            this.SceneGameData = new SceneGameData();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public TestUI UI;

        [SerializeField]
        public GameSceneType StartupScene;

        [SerializeField]
        public SceneGameData SceneGameData;

        public void Awake()
        {
            GameSystem.Instantiate();
            GameSystem.Instance.SetInit(this);
            GameSystem.Instance.Initialize();

            GameSystem.Instance.Transition(this.StartupScene);
        }
    }
}
