namespace CarbonCore.Utils.Unity.Editor.GUI
{
    using System;

    using UnityEditor;

    using UnityEngine;

    public class LayoutRegion : IDisposable
    {
        private readonly LayoutRegionSettings settings;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public LayoutRegion(LayoutRegionSettings settings)
        {
            this.settings = settings;

            if (this.settings.IsHorizontal)
            {
                EditorGUILayout.BeginHorizontal();
            }
            else
            {
                EditorGUILayout.BeginVertical();
            }

            if (settings.FlexibleStart)
            {
                GUILayout.FlexibleSpace();
            }
            else if (settings.MarginStart > 0f)
            {
                GUILayout.Space(settings.MarginStart);
            }
        }

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

            return new LayoutRegion(settings);
        }

        public static LayoutRegion EndAligned(float marginEnd = 0f, bool isHorizontal = false)
        {
            var settings = new LayoutRegionSettings
            {
                FlexibleStart = true,
                MarginEnd = marginEnd,
                IsHorizontal = isHorizontal
            };

            return new LayoutRegion(settings);
        }

        public static LayoutRegion Centered(bool isHorizontal = false)
        {
            var settings = new LayoutRegionSettings
            {
                FlexibleStart = true,
                FlexibleEnd = true,
                IsHorizontal = isHorizontal
            };

            return new LayoutRegion(settings);
        }

        public static LayoutRegion Default(float marginStart = 0f, float marginEnd = 0f, bool isHorizontal = false)
        {
            var settings = new LayoutRegionSettings
                               {
                                   MarginStart = marginStart,
                                   MarginEnd = marginEnd,
                                   IsHorizontal = isHorizontal
                               };

            return new LayoutRegion(settings);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
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
            }
        }
    }
}
