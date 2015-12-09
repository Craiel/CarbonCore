namespace CarbonCore.Utils.Unity.Editor.GUI
{
    using UnityEditor;

    using UnityEngine;

    public static class Splitter
    {
        private static readonly GUIStyle Style;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        static Splitter()
        {
            Style = new GUIStyle
                        {
                            normal = { background = EditorGUIUtility.whiteTexture },
                            stretchWidth = true,
                            margin = new RectOffset(0, 0, 7, 7)
                        };
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Draw(float margin = 8f)
        {
            GUILayout.Space(margin);
            GUILayout.Box(string.Empty, Style, GUILayout.ExpandWidth(true), GUILayout.Height(1));
            GUILayout.Space(margin);
        }
    }
}