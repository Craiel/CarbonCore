namespace CarbonCore.Unity.Editor.Utils.UserInterface
{
    using UnityEngine;

    public static class GuiUtils
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------

        // Some helper functions to draw buttons that are only as big as their text
        public static bool ButtonClamped(Vector2 drawPosition, string text, GUIStyle style, out Vector2 size)
        {
            var content = new GUIContent(text);
            size = style.CalcSize(content);
            var rect = new Rect(drawPosition, size);
            return GUI.Button(rect, text, style);
        }

        public static bool ToggleClamped(Vector2 drawPosition, bool state, string text, GUIStyle style, out Vector2 size)
        {
            var content = new GUIContent(text);
            return ToggleClamped(drawPosition, state, content, style, out size);
        }

        public static bool ToggleClamped(Vector2 drawPosition, bool state, GUIContent content, GUIStyle style, out Vector2 size)
        {
            size = style.CalcSize(content);
            Rect drawRect = new Rect(drawPosition, size);
            return GUI.Toggle(drawRect, state, content, style);
        }

        public static void LabelClamped(Vector2 drawPosition, string text, GUIStyle style, out Vector2 size)
        {
            var content = new GUIContent(text);
            size = style.CalcSize(content);

            Rect drawRect = new Rect(drawPosition, size);
            GUI.Label(drawRect, text, style);
        }
    }
}
