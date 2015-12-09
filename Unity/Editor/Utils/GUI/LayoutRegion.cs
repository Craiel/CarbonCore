namespace CarbonCore.Utils.Unity.Editor.GUI
{
    using UnityEditor;

    using UnityEngine;

    public class LayoutRegion
    {
        private LayoutRegionSettings settings;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static LayoutRegion StartAligned(float marginStart = 0f, bool isHorizontal = false)
        {
            var settings = new LayoutRegionSettings
                               {
                                   FlexibleEnd = true,
                                   MarginStart = marginStart,
                                   IsHorizontal = isHorizontal
                               };

            return new LayoutRegion().Begin(settings);
        }

        public static LayoutRegion EndAligned(float marginEnd = 0f, bool isHorizontal = false)
        {
            var settings = new LayoutRegionSettings
            {
                FlexibleStart = true,
                MarginEnd = marginEnd,
                IsHorizontal = isHorizontal
            };

            return new LayoutRegion().Begin(settings);
        }

        public static LayoutRegion Centered(bool isHorizontal = false)
        {
            var settings = new LayoutRegionSettings
            {
                FlexibleStart = true,
                FlexibleEnd = true,
                IsHorizontal = isHorizontal
            };

            return new LayoutRegion().Begin(settings);
        }

        public static LayoutRegion Default(float marginStart = 0f, float marginEnd = 0f, bool isHorizontal = false)
        {
            var settings = new LayoutRegionSettings
                               {
                                   MarginStart = marginStart,
                                   MarginEnd = marginEnd,
                                   IsHorizontal = isHorizontal
                               };

            return new LayoutRegion().Begin(settings);
        }

        public LayoutRegion Begin(LayoutRegionSettings newSettings)
        {
            this.settings = newSettings;

            if (this.settings.IsHorizontal)
            {
                EditorGUILayout.BeginHorizontal();
            }
            else
            {
                EditorGUILayout.BeginVertical();
            }

            if (this.settings.FlexibleStart)
            {
                GUILayout.FlexibleSpace();
            }
            else if (this.settings.MarginStart > 0f)
            {
                GUILayout.Space(this.settings.MarginStart);
            }

            return this;
        }

        public LayoutRegion End()
        {
            if (this.settings.FlexibleEnd)
            {
                GUILayout.FlexibleSpace();
            }
            else if (this.settings.MarginEnd > 0f)
            {
                GUILayout.Space(this.settings.MarginEnd);
            }

            if (this.settings.IsHorizontal)
            {
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.EndVertical();
            }

            return this;
        }
    }
}
