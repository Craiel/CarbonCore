namespace Assets.Scripts.Data
{
    using global::System;

    using UnityEngine;

    [Serializable]
    public class SceneGameData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public SceneGameData()
        {
            this.EnableAsyncLoading = true;
        }
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [SerializeField]
        public bool EnableAsyncLoading;

        [SerializeField]
        public bool EnableInstantiation;
    }
}
