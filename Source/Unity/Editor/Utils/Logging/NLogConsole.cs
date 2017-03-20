namespace CarbonCore.Unity.Editor.Utils.Logging
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Unity.Utils.Logic.Logging;

    using UnityEditor;

    using UnityEngine;

    public partial class NLogConsole : EditorWindow
    {
        private readonly IList<CountedLog> renderList = new List<CountedLog>();

        private Texture2D errorIcon;
        private Texture2D warningIcon;
        private Texture2D messageIcon;
        private Texture2D smallErrorIcon;
        private Texture2D smallWarningIcon;
        private Texture2D smallMessageIcon;

        private GUIStyle entryStyleBackEven;
        private GUIStyle entryStyleBackOdd;

        private Color sizerLineColor;

        private Vector2 logListScrollPosition;
        private Vector2 logDetailsScrollPosition;

        private string filterRegexText;
        private System.Text.RegularExpressions.Regex filterRegex;
        private bool showError = true;
        private bool showWarning = true;
        private bool showInfo = true;
        private bool hasChanged = true;
        private bool forceRepaint;
        private bool clearOnPlay = true;
        private bool wasPlaying;
        private bool collapse;
        private bool scrollFollowMessages;
        private bool showFrameSource = true;
        private float currentTopPaneHeight = 200;
        private float dividerHeight = 5;
        private int selectedLogIndex = -1;
        private int selectedCallstackFrame = -1;
        private float logListMaxWidth;
        private float logListLineHeight;
        private float collapseBadgeMaxWidth;
        private bool resize;
        private Rect cursorChangeRect;

        private string nameFilter;

        private Vector2 drawPos;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [MenuItem("Window/NLog Console")]
        public static void ShowLogWindow()
        {
            Init();
        }

        public static void Init()
        {
            var window = CreateInstance<NLogConsole>();
            window.Show();
            window.position = new Rect(200, 200, 400, 300);
            window.currentTopPaneHeight = window.position.height / 2;
        }
        
        public void OnInspectorUpdate()
        {
            if (this.hasChanged)
            {
                this.Repaint();
            }
        }

        public void OnEnable()
        {
            // Connect to or create the backend
            if (!NLogInterceptor.IsInstanceActive)
            {
                NLogInterceptor.InstantiateAndInitialize();
                NLogInterceptor.Instance.OnLogChanged += () => this.hasChanged = true;
            }
            
            titleContent.text = "NLog Console";

            EditorApplication.playmodeStateChanged += this.OnPlaymodeStateChanged;

            this.ClearSelectedMessage();

            this.smallErrorIcon = EditorGUIUtility.FindTexture("d_console.erroricon.sml");
            this.smallWarningIcon = EditorGUIUtility.FindTexture("d_console.warnicon.sml");
            this.smallMessageIcon = EditorGUIUtility.FindTexture("d_console.infoicon.sml");

            this.errorIcon = this.smallErrorIcon;
            this.warningIcon = this.smallWarningIcon;
            this.messageIcon = this.smallMessageIcon;
            this.hasChanged = true;
            this.Repaint();
        }
        
        public void OnGUI()
        {
            // Set up the basic style, based on the Unity defaults
            // A bit hacky, but means we don't have to ship an editor guistyle and can fit in to pro and free skins
            Color defaultLineColor = GUI.backgroundColor;
            GUIStyle unityLogLineEven = null;
            GUIStyle unityLogLineOdd = null;
            GUIStyle unitySmallLogLine = null;

            foreach (var style in GUI.skin.customStyles)
            {
                if (style.name == "CN EntryBackEven")
                {
                    unityLogLineEven = style;
                }
                else if (style.name == "CN EntryBackOdd")
                {
                    unityLogLineOdd = style;
                }
                else if (style.name == "CN StatusInfo")
                {
                    unitySmallLogLine = style;
                }
            }

            this.entryStyleBackEven = new GUIStyle(unitySmallLogLine)
                                          {
                                              normal = unityLogLineEven.normal,
                                              margin = new RectOffset(0, 0, 0, 0),
                                              border = new RectOffset(0, 0, 0, 0),
                                              fixedHeight = 0
                                          };


            this.entryStyleBackOdd = new GUIStyle(entryStyleBackEven) { normal = unityLogLineOdd.normal };


            this.sizerLineColor = new Color(defaultLineColor.r * 0.5f, defaultLineColor.g * 0.5f, defaultLineColor.b * 0.5f);

            // GUILayout.BeginVertical(GUILayout.Height(topPanelHeaderHeight), GUILayout.MinHeight(topPanelHeaderHeight));
            this.ResizeTopPane();
            this.drawPos = Vector2.zero;
            this.DrawToolbar();
            this.DrawFilter();

            this.DrawNames();

            float logPanelHeight = this.currentTopPaneHeight - this.drawPos.y;
            
            this.DrawLogList(logPanelHeight);

            this.drawPos.y += this.dividerHeight;

            this.DrawLogDetails();

            // If we're dirty, do a repaint
            this.hasChanged = false;
            if (this.forceRepaint)
            {
                this.hasChanged = true;
                this.forceRepaint = false;
                this.Repaint();
            }
        }

        /*
        
        

        
        void ToggleShowSource(LogStackFrame frame)
        {
            ShowFrameSource = !ShowFrameSource;
        }

        bool JumpToSource(LogStackFrame frame)
        {
            if (frame.FileName != null)
            {
                var filename = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), frame.FileName);
                if (System.IO.File.Exists(filename))
                {
                    if (UnityEditorInternal.InternalEditorUtility.OpenFileAtLineExternal(frame.FileName, frame.LineNumber))
                        return true;
                }
            }

            return false;
        }

               
        List<string> GetChannels()
        {
            if (this.HasChanged)
            {
                CurrentChannels = EditorLogger.CopyChannels();
            }

            var categories = CurrentChannels;

            var channelList = new List<string> { "All", "No Channel" };
            channelList.AddRange(categories);
            return channelList;
        }
        
        //Cache for GetSourceForFrame
        string SourceLines;
        LogStackFrame SourceLinesFrame;

        /// <summary>
        ///If the frame has a valid filename, get the source string for the code around the frame
        ///This is cached, so we don't keep getting it.
        /// </summary>
        string GetSourceForFrame(LogStackFrame frame)
        {
            if (SourceLinesFrame == frame)
            {
                return SourceLines;
            }


            if (frame.FileName == null)
            {
                return "";
            }
            var filename = System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), frame.FileName);
            if (!System.IO.File.Exists(filename))
            {
                return "";
            }

            int lineNumber = frame.LineNumber - 1;
            int linesAround = 3;
            var lines = System.IO.File.ReadAllLines(filename);
            var firstLine = Mathf.Max(lineNumber - linesAround, 0);
            var lastLine = Mathf.Min(lineNumber + linesAround + 1, lines.Count());

            SourceLines = "";
            if (firstLine != 0)
            {
                SourceLines += "...\n";
            }
            for (int c1 = firstLine; c1 < lastLine; c1++)
            {
                string str = lines[c1] + "\n";
                if (c1 == lineNumber)
                {
                    str = "<color=#ff0000ff>" + str + "</color>";
                }
                SourceLines += str;
            }
            if (lastLine != lines.Count())
            {
                SourceLines += "...\n";
            }

            SourceLinesFrame = frame;
            return SourceLines;
        }
        

        Vector2 LogListScrollPosition;
        Vector2 LogDetailsScrollPosition;

        Texture2D ErrorIcon;
        Texture2D WarningIcon;
        Texture2D MessageIcon;
        Texture2D SmallErrorIcon;
        Texture2D SmallWarningIcon;
        Texture2D SmallMessageIcon;

        bool ShowTimes = true;
        bool Collapse;
        bool ScrollFollowMessages;
        
        bool Resize;
        Rect CursorChangeRect;
        bool HasChanged;
        bool ForceRepaint;
        

        

        const double DoubleClickInterval = 0.3f;
        
        List<UberLogger.LogInfo> CurrentLogList = new List<UberLogger.LogInfo>();
        HashSet<string> CurrentChannels = new HashSet<string>();

        //Standard unity pro colours
        Color SizerLineColour;

        
        string CurrentName = null;
        
        
        int SelectedCallstackFrame = 0;
        bool ShowFrameSource = false;

                
        */
    }
}
