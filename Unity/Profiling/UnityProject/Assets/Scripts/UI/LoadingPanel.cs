namespace Assets.Scripts.UI
{
    using System;

    using Assets.Scripts.Enums;

    using CarbonCore.Utils.Unity.Logic.Resource;

    using UnityEngine;
    using UnityEngine.UI;

    public class LoadingPanel : ScenePanel
    {
        private const string LoadingDetailedTextFormat = "Loading {0} remaining\n{1} +{2}";
        private const string LoadingGenericTextFormat = "Loading ...";

        private int maxProgress;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override GameSceneType Type
        {
            get
            {
                // Should never get queried
                throw new InvalidOperationException();
            }
        }

        [SerializeField]
        public GameObject FullScreenDisplay;

        [SerializeField]
        public GameObject Background;

        [SerializeField]
        public Image ProgressBar;

        [SerializeField]
        public Text ProgressText;

        [SerializeField]
        public Text ProgressDetailText;
        
        public override void Show()
        {
            this.maxProgress = 0;
            
            base.Show();
        }
        
        public void Update()
        {
            bool showBackground = this.FullScreenDisplay.activeInHierarchy;
            this.Background.SetActive(showBackground);

            int pendingCount = ResourceProvider.Instance.PendingForLoad + ResourceProvider.Instance.RequestPool.ActiveRequestCount + BundleProvider.Instance.PendingForLoad;
            if (pendingCount > this.maxProgress)
            {
                this.maxProgress = pendingCount;
            }

            float progress = 0f;
            if (this.maxProgress > 0)
            {
                progress = 1 - (pendingCount / (float)this.maxProgress);
            }

            this.ProgressBar.transform.localScale = new Vector3(progress, 1, 1);
            this.ProgressText.text = string.Format("{0:#,0}%", progress * 100);

            ResourceLoadRequest resourceRequest = ResourceProvider.Instance.RequestPool.GetFirstActiveRequest();
            if (resourceRequest != null)
            {
                string text = string.Format(LoadingDetailedTextFormat, pendingCount, resourceRequest.Info.Key, ResourceProvider.Instance.RequestPool.ActiveRequestCount);
                this.ProgressDetailText.text = text;
            }
            else if (BundleProvider.Instance.CurrentRequest != null)
            {
                string text = string.Format(LoadingDetailedTextFormat, pendingCount, BundleProvider.Instance.CurrentRequest.Key, 0);
                this.ProgressDetailText.text = text;
            }
            else
            {
                this.ProgressDetailText.text = LoadingGenericTextFormat;
            }
        }
    }
}
