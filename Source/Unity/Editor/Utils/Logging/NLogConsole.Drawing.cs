namespace CarbonCore.Unity.Editor.Utils.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    using NLog;

    using Unity.Utils.Logic.Logging;

    using UnityEditor;

    using UnityEngine;

    using UserInterface;

    public partial class NLogConsole
    {
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DrawToolbar()
        {
            var toolbarStyle = EditorStyles.toolbarButton;

            Vector2 elementSize;
            if (GuiUtils.ButtonClamped(this.drawPos, "Clear", EditorStyles.toolbarButton, out elementSize))
            {
                NLogInterceptor.Instance.Clear();
            }

            drawPos.x += elementSize.x;
            this.clearOnPlay = GuiUtils.ToggleClamped(this.drawPos, this.clearOnPlay, "Clear On Play", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;
            NLogInterceptor.Instance.PauseOnError = GuiUtils.ToggleClamped(this.drawPos, NLogInterceptor.Instance.PauseOnError, "Error Pause", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;
            
            drawPos.x += elementSize.x;
            var newCollapse = GuiUtils.ToggleClamped(this.drawPos, this.collapse, "Collapse", EditorStyles.toolbarButton, out elementSize);
            if (newCollapse != this.collapse)
            {
                this.forceRepaint = true;
                this.collapse = newCollapse;
                selectedLogIndex = -1;
            }

            drawPos.x += elementSize.x;

            scrollFollowMessages = GuiUtils.ToggleClamped(this.drawPos, scrollFollowMessages, "Follow", EditorStyles.toolbarButton, out elementSize);
            drawPos.x += elementSize.x;

            var errorToggleContent = new GUIContent(NLogInterceptor.Instance.GetCount(LogLevel.Error).ToString(), smallErrorIcon);
            var warningToggleContent = new GUIContent(NLogInterceptor.Instance.GetCount(LogLevel.Warn).ToString(), smallWarningIcon);
            var messageToggleContent = new GUIContent(NLogInterceptor.Instance.GetCount(LogLevel.Info).ToString(), smallMessageIcon);

            float totalErrorButtonWidth = toolbarStyle.CalcSize(errorToggleContent).x + toolbarStyle.CalcSize(warningToggleContent).x + toolbarStyle.CalcSize(messageToggleContent).x;

            float errorIconX = position.width - totalErrorButtonWidth;
            if (errorIconX > drawPos.x)
            {
                drawPos.x = errorIconX;
            }

            var showErrors = GuiUtils.ToggleClamped(this.drawPos, this.showError, errorToggleContent, toolbarStyle, out elementSize);
            drawPos.x += elementSize.x;
            var showWarnings = GuiUtils.ToggleClamped(this.drawPos, this.showWarning, warningToggleContent, toolbarStyle, out elementSize);
            drawPos.x += elementSize.x;
            var showMessages = GuiUtils.ToggleClamped(this.drawPos, this.showInfo, messageToggleContent, toolbarStyle, out elementSize);
            drawPos.x += elementSize.x;

            drawPos.y += elementSize.y;
            drawPos.x = 0;

            // If the errors/warning to show has changed, clear the selected message
            if (showErrors != this.showError || showWarnings != this.showWarning || showMessages != this.showInfo)
            {
                ClearSelectedMessage();
                this.forceRepaint = true;
            }

            this.showWarning = showWarnings;
            this.showInfo = showMessages;
            this.showError = showErrors;
        }
        
        private void DrawNames()
        {
            var names = new List<string> { "All", "No Channel" };
            names.AddRange(NLogInterceptor.Instance.Names);

            int currentNameIndex = 0;
            for (int i = 0; i < names.Count; i++)
            {
                if (names[i] == this.nameFilter)
                {
                    currentNameIndex = i;
                    break;
                }
            }

            var content = new GUIContent("S");
            var size = GUI.skin.button.CalcSize(content);
            var drawRect = new Rect(drawPos, new Vector2(position.width, size.y));
            currentNameIndex = GUI.SelectionGrid(drawRect, currentNameIndex, names.ToArray(), names.Count);
            if (this.nameFilter != names[currentNameIndex])
            {
                this.nameFilter = names[currentNameIndex];
                ClearSelectedMessage();
                this.forceRepaint = true;
            }

            drawPos.y += size.y;
        }
        
        private void DrawLogList(float height)
        {
            var oldColor = GUI.backgroundColor;
            
            float buttonY;
            
            var collapseBadgeStyle = EditorStyles.miniButton;
            var logLineStyle = entryStyleBackEven;

            // If we've been marked dirty, we need to recalculate the elements to be displayed
            if (this.hasChanged)
            {
                logListMaxWidth = 0;
                logListLineHeight = 0;
                collapseBadgeMaxWidth = 0;
                this.renderList.Clear();

                // When collapsed, count up the unique elements and use those to display
                if (collapse)
                {
                    var collapsedLines = new Dictionary<string, CountedLog>();
                    var collapsedLinesList = new List<CountedLog>();

                    foreach (var log in NLogInterceptor.Instance.Events)
                    {
                        if (!FilterLog(log))
                        {
                            var matchString = string.Concat(log.Message, "!$", log.Level, "!$", log.LoggerName);

                            CountedLog countedLog;
                            if (collapsedLines.TryGetValue(matchString, out countedLog))
                            {
                                countedLog.Count++;
                            }
                            else
                            {
                                countedLog = new CountedLog(log, 1);
                                collapsedLines.Add(matchString, countedLog);
                                collapsedLinesList.Add(countedLog);
                            }
                        }
                    }

                    foreach (var countedLog in collapsedLinesList)
                    {
                        var content = GetLogLineGUIContent(countedLog.Log);
                        this.renderList.Add(countedLog);
                        var logLineSize = logLineStyle.CalcSize(content);
                        logListMaxWidth = Mathf.Max(logListMaxWidth, logLineSize.x);
                        logListLineHeight = Mathf.Max(logListLineHeight, logLineSize.y);

                        var collapseBadgeContent = new GUIContent(countedLog.Count.ToString());
                        var collapseBadgeSize = collapseBadgeStyle.CalcSize(collapseBadgeContent);
                        collapseBadgeMaxWidth = Mathf.Max(collapseBadgeMaxWidth, collapseBadgeSize.x);
                    }
                }
                else
                {
                    // If we're not collapsed, display everything in order
                    foreach (var log in NLogInterceptor.Instance.Events)
                    {
                        if (!FilterLog(log))
                        {
                            var content = GetLogLineGUIContent(log);
                            this.renderList.Add(new CountedLog(log, 1));
                            var logLineSize = logLineStyle.CalcSize(content);
                            logListMaxWidth = Mathf.Max(logListMaxWidth, logLineSize.x);
                            logListLineHeight = Mathf.Max(logListLineHeight, logLineSize.y);
                        }
                    }
                }

                logListMaxWidth += collapseBadgeMaxWidth;
            }

            var scrollRect = new Rect(drawPos, new Vector2(position.width, height));
            float lineWidth = Mathf.Max(logListMaxWidth, scrollRect.width);

            var contentRect = new Rect(0, 0, lineWidth, this.renderList.Count * logListLineHeight);
            Vector2 lastScrollPosition = logListScrollPosition;
            logListScrollPosition = GUI.BeginScrollView(scrollRect, logListScrollPosition, contentRect);

            // If we're following the messages but the user has moved, cancel following
            if (scrollFollowMessages)
            {
                if (lastScrollPosition.y - logListScrollPosition.y > logListLineHeight)
                {
                    scrollFollowMessages = false;
                }
            }

            float logLineX = collapseBadgeMaxWidth;

            // Render all the elements
            int firstRenderLogIndex = (int)(logListScrollPosition.y / logListLineHeight);
            int lastRenderLogIndex = firstRenderLogIndex + (int)(height / logListLineHeight);

            firstRenderLogIndex = Mathf.Clamp(firstRenderLogIndex, 0, this.renderList.Count);
            lastRenderLogIndex = Mathf.Clamp(lastRenderLogIndex, 0, this.renderList.Count);
            buttonY = firstRenderLogIndex * logListLineHeight;

            for (int renderLogIndex = firstRenderLogIndex; renderLogIndex < lastRenderLogIndex; renderLogIndex++)
            {
                var countedLog = this.renderList[renderLogIndex];
                var log = countedLog.Log;
                logLineStyle = (renderLogIndex % 2 == 0) ? entryStyleBackEven : entryStyleBackOdd;
                if (renderLogIndex == selectedLogIndex)
                {
                    GUI.backgroundColor = new Color(0.5f, 0.5f, 1);
                }
                else
                {
                    GUI.backgroundColor = Color.white;
                }

                // Make all messages single line
                var content = GetLogLineGUIContent(log);
                var drawRect = new Rect(logLineX, buttonY, contentRect.width, logListLineHeight);
                if (GUI.Button(drawRect, content, logLineStyle))
                {
                    // Select a message, or jump to source if it's double-clicked
                    if (renderLogIndex != selectedLogIndex)
                    {
                        selectedLogIndex = renderLogIndex;
                        selectedCallstackFrame = -1;
                    }
                }

                if (collapse)
                {
                    var collapseBadgeContent = new GUIContent(countedLog.Count.ToString());
                    var collapseBadgeSize = collapseBadgeStyle.CalcSize(collapseBadgeContent);
                    var collapseBadgeRect = new Rect(0, buttonY, collapseBadgeSize.x, collapseBadgeSize.y);
                    GUI.Button(collapseBadgeRect, collapseBadgeContent, collapseBadgeStyle);
                }

                buttonY += logListLineHeight;
            }

            // If we're following the log, move to the end
            if (scrollFollowMessages && this.renderList.Count > 0)
            {
                logListScrollPosition.y = ((this.renderList.Count + 1) * logListLineHeight) - scrollRect.height;
            }

            GUI.EndScrollView();
            drawPos.y += height;
            drawPos.x = 0;
            GUI.backgroundColor = oldColor;
        }

        private GUIContent GetLogLineGUIContent(LogEventInfo log)
        {
            var content = new GUIContent(log.FormattedMessage.Replace("\n", " "), this.GetIconForLog(log));
            return content;
        }

        private Texture2D GetIconForLog(LogEventInfo log)
        {
            if (log.Level == LogLevel.Error)
            {
                return errorIcon;
            }

            if (log.Level == LogLevel.Warn)
            {
                return warningIcon;
            }

            return messageIcon;
        }

        private void DrawFilter()
        {
            Vector2 size;
            GuiUtils.LabelClamped(this.drawPos, "Filter Regex", GUI.skin.label, out size);
            drawPos.x += size.x;
            
            bool clearFilter = false;
            if (GuiUtils.ButtonClamped(this.drawPos, "Clear", GUI.skin.button, out size))
            {
                clearFilter = true;

                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
            }

            drawPos.x += size.x;

            var drawRect = new Rect(drawPos, new Vector2(position.width - drawPos.x, size.y));
            string newFilter = EditorGUI.TextArea(drawRect, this.filterRegexText);

            if (clearFilter)
            {
                this.filterRegexText = null;
                this.filterRegex = null;
            }

            // If the filter has changed, invalidate our currently selected message
            if (this.filterRegexText != newFilter)
            {
                ClearSelectedMessage();
                this.filterRegexText = newFilter;
                this.filterRegex = new Regex(this.filterRegexText, RegexOptions.IgnoreCase);
                this.forceRepaint = true;
            }

            drawPos.y += size.y;
            drawPos.x = 0;
        }

        private void DrawStackTrace(LogEventInfo log, List<GUIContent> target, GUIStyle style, GUIStyle sourceStyle, ref float contentHeight, ref float contentWidth, ref float lineHeight)
        {
            if (log.StackTrace == null)
            {
                var unityStackException = log.Exception as UnityStackTraceException;
                if (unityStackException != null)
                {
                    string[] lines = unityStackException.StackContents.Split(
                        new[] { "\n" },
                        StringSplitOptions.RemoveEmptyEntries);
                    for (var i = 0; i < lines.Length; i++)
                    {
                        var content = new GUIContent(lines[i]);
                        var contentSize = style.CalcSize(content);
                        contentHeight += contentSize.y;
                        lineHeight = Mathf.Max(lineHeight, contentSize.y);
                        contentWidth = Mathf.Max(contentSize.x, contentWidth);
                        target.Add(content);
                    }
                }

                return;
            }
            
            for (int i = 0; i < log.StackTrace.FrameCount; i++)
            {
                var frame = log.StackTrace.GetFrame(i);
                var methodName = frame.GetMethod().Name;
                if (!string.IsNullOrEmpty(methodName))
                {
                    var content = new GUIContent(methodName);
                    target.Add(content);

                    var contentSize = style.CalcSize(content);
                    contentHeight += contentSize.y;
                    lineHeight = Mathf.Max(lineHeight, contentSize.y);
                    contentWidth = Mathf.Max(contentSize.x, contentWidth);
                    if (showFrameSource && i == selectedCallstackFrame)
                    {
                        var sourceContent = new GUIContent(frame.ToString());
                        var sourceSize = sourceStyle.CalcSize(sourceContent);
                        contentHeight += sourceSize.y;
                        contentWidth = Mathf.Max(sourceSize.x, contentWidth);
                    }
                }
            }
        }

        /// <summary>
        /// The bottom of the panel - details of the selected log
        /// </summary>
        private void DrawLogDetails()
        {
            var oldColor = GUI.backgroundColor;

            this.selectedLogIndex = Mathf.Clamp(this.selectedLogIndex, 0, this.renderList.Count);

            if (this.renderList.Count > 0 && this.selectedLogIndex >= 0)
            {
                var countedLog = this.renderList[this.selectedLogIndex];
                var log = countedLog.Log;
                var logLineStyle = entryStyleBackEven;

                var sourceStyle = new GUIStyle(GUI.skin.textArea) { richText = true };

                var drawRect = new Rect(drawPos, new Vector2(position.width - drawPos.x, position.height - drawPos.y));

                var detailLines = new List<GUIContent>();
                float contentHeight = 0;
                float contentWidth = 0;
                float lineHeight = 0;

                // Work out the content we need to show, and the sizes
                this.DrawStackTrace(log, detailLines, logLineStyle, sourceStyle, ref contentHeight, ref contentWidth, ref lineHeight);

                // Render the content
                var contentRect = new Rect(0, 0, Mathf.Max(contentWidth, drawRect.width), contentHeight);

                logDetailsScrollPosition = GUI.BeginScrollView(drawRect, logDetailsScrollPosition, contentRect);

                float lineY = 0;
                for (int i = 0; i < detailLines.Count; i++)
                {
                    var lineContent = detailLines[i];
                    if (lineContent != null)
                    {
                        logLineStyle = (i % 2 == 0) ? entryStyleBackEven : entryStyleBackOdd;
                        if (i == selectedCallstackFrame)
                        {
                            GUI.backgroundColor = new Color(0.5f, 0.5f, 1);
                        }
                        else
                        {
                            GUI.backgroundColor = Color.white;
                        }
                        
                        var lineRect = new Rect(0, lineY, contentRect.width, lineHeight);

                        // Handle clicks on the stack frame
                        if (GUI.Button(lineRect, lineContent, logLineStyle))
                        {
                            if (i == selectedCallstackFrame)
                            {
                                if (Event.current.button == 1)
                                {
                                    showFrameSource = !this.showFrameSource;
                                    Repaint();
                                }
                            }
                            else
                            {
                                selectedCallstackFrame = i;
                            }
                        }

                        lineY += lineHeight;

                        // Show the source code if needed
                        if (showFrameSource && i == selectedCallstackFrame)
                        {
                            GUI.backgroundColor = Color.white;

                            GUIContent sourceContent;
                            if (log.StackTrace != null)
                            {
                                var frame = log.StackTrace.GetFrame(i);
                                sourceContent = new GUIContent(frame.ToString());
                            }
                            else
                            {
                                sourceContent = new GUIContent(detailLines[i]);
                            }

                            var sourceSize = sourceStyle.CalcSize(sourceContent);
                            var sourceRect = new Rect(0, lineY, contentRect.width, sourceSize.y);

                            GUI.Label(sourceRect, sourceContent, sourceStyle);
                            lineY += sourceSize.y;
                        }
                    }
                }

                GUI.EndScrollView();
            }

            GUI.backgroundColor = oldColor;
        }
    }
}
