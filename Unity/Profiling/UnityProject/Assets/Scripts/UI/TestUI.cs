namespace Assets.Scripts.UI
{
    using System;

    using Assets.Scripts.Enums;
    using Assets.Scripts.Game;

    using CarbonCore.Utils.Collections;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Unity.Logic;

    using UnityEngine;
    using UnityEngine.UI;

    public class TestUI : MonoBehaviour
    {
        private readonly ExtendedDictionary<GameSceneType, ScenePanel> panelTypeMap;

        private ScenePanel activePanel;

        private IntervalTrigger fpsUpdateTrigger;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TestUI()
        {
            this.panelTypeMap = new ExtendedDictionary<GameSceneType, ScenePanel> { EnableReverseLookup = true };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public ScenePanel LoadingPanel;

        [SerializeField]
        public Text VersionText;

        [SerializeField]
        public Text FpsText;

        [SerializeField]
        public ScenePanel[] Panels;

        public void Awake()
        {
            // Ensure the game ui never gets destroyed
            DontDestroyOnLoad(this.gameObject);

            this.panelTypeMap.Clear();
            if (this.Panels == null || this.Panels.Length <= 0)
            {
                Diagnostic.Error("No UI Panels defined");
                return;
            }

            foreach (ScenePanel panel in this.Panels)
            {
                this.RegisterPanel(panel);
            }

            this.VersionText.text = string.Format("{0} {1}", Constants.GameName, Constants.Version);

            this.fpsUpdateTrigger = IntervalTrigger.Create(Constants.FpsUpdateInterval, this.UpdateFpsDisplay);
        }

        public void Update()
        {
            this.fpsUpdateTrigger.Update(Time.time);

            if (GameSystem.Instance.InTransition)
            {
                foreach (GameSceneType type in this.panelTypeMap.Keys)
                {
                    this.panelTypeMap[type].Hide();
                }

                this.LoadingPanel.Show();

                // Nothing else to do in transition
                return;
            }

            this.LoadingPanel.Hide();
            if (this.NeedPanelUpdate())
            {
                if (this.activePanel != null)
                {
                    this.activePanel.Hide();
                }

                this.activePanel = this.panelTypeMap[GameSystem.Instance.ActiveSceneType.Value];
                this.activePanel.Show();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool NeedPanelUpdate()
        {
            if (GameSystem.Instance.ActiveSceneType == null)
            {
                // Nothing to update to
                return false;
            }

            if (this.activePanel == null)
            {
                return true;
            }

            if (this.activePanel.Type != GameSystem.Instance.ActiveSceneType.Value)
            {
                return true;
            }

            return false;
        }

        private void RegisterPanel(ScenePanel panel)
        {
            if (this.panelTypeMap.ContainsKey(panel.Type))
            {
                Diagnostic.Error("Duplicate UI Panel for {0}: {1} and {2}", panel.Type, panel, this.panelTypeMap[panel.Type]);
                return;
            }

            this.panelTypeMap.Add(panel.Type, panel);
        }

        private void UpdateFpsDisplay(float currentTime, IntervalTrigger trigger)
        {
            float fps = 0;
            if (Math.Abs(Time.deltaTime) > float.Epsilon)
            {
                fps = 1.0f / Time.deltaTime;
            }

            this.FpsText.text = string.Format(Constants.FpsFormat, fps);
        }
    }
}
