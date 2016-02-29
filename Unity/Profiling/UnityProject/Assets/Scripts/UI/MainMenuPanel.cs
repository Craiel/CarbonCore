namespace Assets.Scripts.UI
{
    using Assets.Scripts.Enums;
    using Assets.Scripts.Tests;

    using UnityEngine;
    using UnityEngine.UI;

    public class MainMenuPanel : ScenePanel
    {
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

        [SerializeField]
        public Button GeneralTestButton;

        [SerializeField]
        public Button BehaviorTreeTestButton;

        [SerializeField]
        public Button DictionaryTestButton;

        [SerializeField]
        public Button JsonTestButton;

        public void Awake()
        {
            this.GeneralTestButton.onClick.AddListener(this.OnGeneralTest);
            this.BehaviorTreeTestButton.onClick.AddListener(this.OnBehaviorTreeTest);
            this.DictionaryTestButton.onClick.AddListener(this.OnDictionaryTest);
            this.JsonTestButton.onClick.AddListener(this.OnJsonTest);
        }

        public void Update()
        {
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnGeneralTest()
        {
            TestController.Instance.EnableGeneralTests = !TestController.Instance.EnableGeneralTests;
        }

        private void OnBehaviorTreeTest()
        {
            TestController.Instance.EnableBehaviorTreeTests = !TestController.Instance.EnableBehaviorTreeTests;
        }

        private void OnDictionaryTest()
        {
            TestController.Instance.EnableDictionaryTests = !TestController.Instance.EnableDictionaryTests;
        }

        private void OnJsonTest()
        {
            TestController.Instance.EnableJsonTests = !TestController.Instance.EnableJsonTests;
        }
    }
}
