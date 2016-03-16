namespace CarbonCore.Unity.Editor.Utils
{
    using System.Collections.Generic;
    using System.Linq;

    public static class AssetUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static IDictionary<string, UnityEngine.Object> FindAssets(string filter, string[] folders, bool filesOnly = true, IList<string> ignoreFilters = null)
        {
            var results = new Dictionary<string, UnityEngine.Object>();

            string[] assets = UnityEditor.AssetDatabase.FindAssets(filter, folders);
            foreach (string asset in assets)
            {
                string path = UnityEditor.AssetDatabase.GUIDToAssetPath(asset);
                if (ignoreFilters != null && ignoreFilters.Any(x => path.Contains(x)))
                {
                    continue;
                }

                UnityEngine.Object loadedAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
                if (loadedAsset == null)
                {
                    continue;
                }

                if (results.ContainsKey(path))
                {
                    continue;
                }

                if (filesOnly && !System.IO.File.Exists(path))
                {
                    continue;
                }

                results.Add(path, loadedAsset);
            }

            return results;
        }
    }
}
