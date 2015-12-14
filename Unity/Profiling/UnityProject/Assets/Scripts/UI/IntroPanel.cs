namespace Assets.Scripts.UI
{
    using Assets.Scripts.Enums;
    using Assets.Scripts.Game;

    using UnityEngine;

    // Placeholder...
    public class IntroPanel : ScenePanel
    {
        private float startupTime;

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

        public void Awake()
        {
            this.startupTime = Time.time;
        }

        public void Update()
        {
            if (GameSystem.Instance.InTransition)
            {
                return;
            }

            if (Time.time > this.startupTime + 5f || Input.GetMouseButtonDown(0))
            {
                GameSystem.Instance.Transition(GameSceneType.MainMenu);
            }
        }
    }
}
