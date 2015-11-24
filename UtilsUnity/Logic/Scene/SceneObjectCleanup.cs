namespace CarbonCore.Utils.Unity.Logic.Scene
{
    using System;

    using UnityEngine;

    public class SceneObjectCleanup : MonoBehaviour
    {
        private const float CleanupInterval = 0.1f;

        private float lastCleanup;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action OnCleanup;

        public void Update()
        {
            if (Time.time > this.lastCleanup + CleanupInterval)
            {
                if (this.OnCleanup != null)
                {
                    this.OnCleanup();
                }

                this.lastCleanup = Time.time;
            }
        }
    }
}
