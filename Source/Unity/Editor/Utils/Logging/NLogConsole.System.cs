namespace CarbonCore.Unity.Editor.Utils.Logging
{
    using NLog;

    using Unity.Utils.Logic.Logging;

    using UnityEditor;

    using UnityEngine;

    public partial class NLogConsole
    {
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool FilterLog(LogEventInfo log)
        {
            if (this.filterRegex == null)
            {
                return false;
            }

            if (log.LoggerName == this.nameFilter 
                || this.nameFilter == "All" 
                || (this.nameFilter == "No Name" && string.IsNullOrEmpty(log.LoggerName)))
            {
                if ((log.Level == LogLevel.Info && this.showInfo)
                   || (log.Level == LogLevel.Warn && this.showWarning)
                   || (log.Level == LogLevel.Error && this.showError))
                {
                    if (this.filterRegex == null || this.filterRegex.IsMatch(log.Message))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        
        private void ClearSelectedMessage()
        {
            this.selectedLogIndex = -1;
            this.selectedCallstackFrame = -1;
            showFrameSource = false;
        }

        private void OnPlaymodeStateChanged()
        {
            if (!this.wasPlaying && EditorApplication.isPlayingOrWillChangePlaymode)
            {
                if (this.clearOnPlay)
                {
                    NLogInterceptor.Instance.Clear();
                }
            }

            this.wasPlaying = EditorApplication.isPlayingOrWillChangePlaymode;
        }

        private void ResizeTopPane()
        {
            // Set up the resize collision rect
            cursorChangeRect = new Rect(0, currentTopPaneHeight, position.width, dividerHeight);

            var oldColor = GUI.color;
            GUI.color = this.sizerLineColor;
            GUI.DrawTexture(cursorChangeRect, EditorGUIUtility.whiteTexture);
            GUI.color = oldColor;
            EditorGUIUtility.AddCursorRect(cursorChangeRect, MouseCursor.ResizeVertical);

            if (Event.current.type == EventType.mouseDown && cursorChangeRect.Contains(Event.current.mousePosition))
            {
                resize = true;
            }

            // If we've resized, store the new size and force a repaint
            if (resize)
            {
                currentTopPaneHeight = Event.current.mousePosition.y;
                cursorChangeRect.Set(cursorChangeRect.x, currentTopPaneHeight, cursorChangeRect.width, cursorChangeRect.height);
                Repaint();
            }

            if (Event.current.type == EventType.MouseUp)
            {
                resize = false;
            }

            currentTopPaneHeight = Mathf.Clamp(currentTopPaneHeight, 100, position.height - 100);
        }

        private class CountedLog
        {
            public CountedLog(LogEventInfo log, int count)
            {
                this.Log = log;
                this.Count = count;
            }

            public LogEventInfo Log { get; private set; }
            public int Count { get; set; }
        }
    }
}
