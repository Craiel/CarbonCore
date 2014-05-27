namespace CarbonCore.GrammarParser.Patching
{
    using System.Text.RegularExpressions;

    using CarbonCore.Utils.Diagnostics;

    public class PatchExpression : Patch
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public PatchExpression(string pattern, string replacement, PatchMode mode = PatchMode.Any, int count = 0, RegexOptions options = RegexOptions.None, string[] fileFilters = null)
            : base(pattern, replacement, mode, count, fileFilters)
        {
            this.Expression = new Regex(pattern, options);
            this.TargetGroup = 1;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Regex Expression { get; private set; }
        public int TargetGroup { get; set; }
        public int? ReplacementGroup { get; set; }
        public string QuickTestString { get; set; }

        public override bool Apply(ref string line)
        {
            if (!string.IsNullOrEmpty(this.QuickTestString) && !line.Contains(this.QuickTestString))
            {
                return false;
            }

            var profileRegion = new ProfileRegion(string.Format("ExpressionPatch {0}", this.Name ?? this.Pattern));
            
            var matches = this.Expression.Matches(line);
            for (int i = 0; i < matches.Count; i++)
            {
                string target = matches[i].Groups[this.TargetGroup].ToString();
                string replacement = this.Replacement;
                if (this.ReplacementGroup != null)
                {
                    replacement = string.Format(replacement, matches[i].Groups[this.ReplacementGroup.Value]);
                }

                line = line.Replace(target, replacement);
                this.ApplyCount++;
            }

            if (matches.Count > 0)
            {
                profileRegion.Tag = this.ApplyCount;
                Profiler.RegionFinish(profileRegion);
            }
            else
            {
                profileRegion.Dispose(true);
            }
            
            return matches.Count > 0;
        }
    }
}
