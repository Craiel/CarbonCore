namespace CarbonCore.Applications.CrystalBuild.Logic.Processors
{
    using System.Collections.Generic;
    using System.Text;
    using System.Text.RegularExpressions;

    using CarbonCore.Utils.IO;

    using CrystalBuild.Contracts.Processors;

    public class CssProcessor : ContentProcessor, ICssProcessor
    {
        private const string IncludeKey = "include";

        private static readonly Regex CommentRegex = new Regex(@"/\*(.*?)\*/");
        private static readonly Regex StyleRegex = new Regex(@"([\.\#])([a-z]+)(.*?)\{([^\}]*)\}", RegexOptions.IgnoreCase);

        private readonly IDictionary<string, CssStyle> styleDictionary;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CssProcessor()
        {
            this.styleDictionary = new Dictionary<string, CssStyle>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void Process(CarbonFile file)
        {
            string content = file.ReadAsString();
            this.AppendLine(content);
        }

        protected override string PostProcessData(string data)
        {
            this.styleDictionary.Clear();
            IList<CssStyle> styles = this.AnalyzeStyleSheets(data);
            foreach (CssStyle style in styles)
            {
                string key = style.Name.ToLowerInvariant();
                if (this.styleDictionary.ContainsKey(key))
                {
                    System.Diagnostics.Trace.TraceError("Duplicate style: " + style.Name);
                    continue;
                }

                this.styleDictionary.Add(key, style);
            }

            // Check all the style's contents next
            foreach (CssStyle style in styles)
            {
                this.CheckStyleContent(style);
            }
            
            return this.FormatStyles(styles);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private string FormatStyles(IEnumerable<CssStyle> styles)
        {
            var builder = new StringBuilder();
            foreach (CssStyle style in styles)
            {
                builder.Append(style.IsStyle ? "." : "#");
                builder.Append(style.Name);
                if (!string.IsNullOrEmpty(style.StyleTarget))
                {
                    builder.Append(style.StyleTarget);
                }

                builder.Append("{");
                foreach (string contentKey in style.Content.Keys)
                {
                    foreach (string entry in style.Content[contentKey])
                    {
                        builder.AppendFormat(" {0}: {1}; ", contentKey, entry);
                    }
                }

                builder.AppendLine("}");
            }

            return builder.ToString();
        }

        private IList<CssStyle> AnalyzeStyleSheets(string data)
        {
            string formattedData = data.Replace("\n", string.Empty).Replace("\r", string.Empty);
            MatchCollection matches = CommentRegex.Matches(formattedData);
            foreach (Match match in matches)
            {
                formattedData = formattedData.Replace(match.Groups[0].Value, string.Empty);
            }

            matches = StyleRegex.Matches(formattedData);
            IList<CssStyle> styles = new List<CssStyle>();
            foreach (Match match in matches)
            {
                var style = new CssStyle
                {
                    IsStyle = match.Groups[1].Value.Trim() == ".",
                    Name = match.Groups[2].Value.Trim(),
                    StyleTarget = match.Groups[3].Value.Trim()
                };

                string content = match.Groups[4].Value.Trim();
                string[] contentSegments = content.Split(';');
                foreach (string segment in contentSegments)
                {
                    if (string.IsNullOrEmpty(segment))
                    {
                        continue;
                    }

                    string[] segmentParts = segment.Split(':');
                    if (segmentParts.Length != 2)
                    {
                        System.Diagnostics.Trace.TraceError("Invalid segment count: " + segment);
                        continue;
                    }
                    
                    style.AddContent(segmentParts[0].Trim(), segmentParts[1].Trim());
                }

                styles.Add(style);
            }

            return styles;
        }

        private void CheckStyleContent(CssStyle style)
        {
            if (style.IsChecked)
            {
                return;
            }

            // Process includes first
            if (style.Content.ContainsKey(IncludeKey))
            {
                foreach (string value in style.Content[IncludeKey])
                {
                    CssStyle includedStyle = this.LocateStyle(value);
                    if (includedStyle == null)
                    {
                        System.Diagnostics.Trace.TraceError("Included style not found, {0} in {1}", value, style.Name);
                        continue;
                    }

                    if (!includedStyle.IsChecked)
                    {
                        this.CheckStyleContent(includedStyle);
                    }

                    this.MergeStyle(includedStyle, style);
                }

                style.Content.Remove(IncludeKey);
            }

            // Now check the style contents for issues
            foreach (string key in style.Content.Keys)
            {
                IList<string> values = style.Content[key];
                foreach (string value in values)
                {
                    if (value.ToLowerInvariant().Contains(".png"))
                    {
                        System.Diagnostics.Trace.TraceWarning("Style contains image reference: " + style.Name);
                    }
                }
            }
            
            style.IsChecked = true;
        }

        private CssStyle LocateStyle(string name)
        {
            string key = name.ToLowerInvariant();
            if (this.styleDictionary.ContainsKey(key) && this.styleDictionary[key].IsStyle)
            {
                return this.styleDictionary[key];
            }

            return null;
        }

        private void MergeStyle(CssStyle source, CssStyle target)
        {
            foreach (string key in source.Content.Keys)
            {
                foreach (string value in source.Content[key])
                {
                    target.AddContent(key, value);
                }
            }
        }

        // -------------------------------------------------------------------
        // internal
        // -------------------------------------------------------------------
        internal class CssStyle
        {
            public CssStyle()
            {
                this.Content = new Dictionary<string, IList<string>>();
            }

            public bool IsStyle { get; set; }
            public bool IsChecked { get; set; }

            public string Name { get; set; }
            public string StyleTarget { get; set; }

            public IDictionary<string, IList<string>> Content { get; private set; }

            public void AddContent(string key, string value)
            {
                if (!this.Content.ContainsKey(key))
                {
                    this.Content.Add(key, new List<string>());
                }

                this.Content[key].Add(value);
            }
        }
    }
}
