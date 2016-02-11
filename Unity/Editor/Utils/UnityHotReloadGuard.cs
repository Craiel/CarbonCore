namespace CarbonCore.Utils.Unity.Editor
{
    using CarbonCore.Utils.Unity.Logic;

    using UnityEditor;

    public class UnityHotReloadGuard : UnitySingleton<UnityHotReloadGuard>
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Initialize()
        {
            base.Initialize();

            EditorApplication.update += this.OnEditorUpdate;
        }

        public override void DestroySingleton()
        {
            EditorApplication.update -= this.OnEditorUpdate;

            base.DestroySingleton();
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnEditorUpdate()
        {
            if (EditorApplication.isPlaying && EditorApplication.isCompiling)
            {
                UnityEngine.Debug.Log("Exiting play mode due to script compilation.");
                EditorApplication.isPlaying = false;
            }
        }
    }
}
