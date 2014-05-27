namespace CarbonCore.GrammarParser.Patching
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;

    public enum PatchMode
    {
        Any,
        Once,
        Count
    }

    public class Patch
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public Patch(string pattern, string replacement, PatchMode mode = PatchMode.Any, int count = 0, string[] fileFilters = null)
        {
            this.Mode = mode;

            this.Pattern = pattern;
            this.Replacement = replacement;
            
            if (fileFilters != null)
            {
                this.FileFilters = new List<string>(fileFilters);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public string Pattern { get; private set; }
        public string Replacement { get; private set; }

        public IList<string> FileFilters { get; private set; }

        public PatchMode Mode { get; private set; }

        public long ApplyCount { get; protected set; }
        public TimeSpan TimeUsed { get; protected set; }

        public virtual bool Apply(ref string line)
        {
            var profileRegion = new ProfileRegion(string.Format("Patch {0}", this.Name ?? this.Pattern));
            
            int index = line.IndexOf(this.Pattern);
            bool didMatch = false;
            while (index >= 0)
            {
                didMatch = true;
                line = string.Concat(line.Substring(0, index), this.Replacement, line.Substring(index + this.Pattern.Length, line.Length - index - this.Pattern.Length));
                this.ApplyCount++;

                index = line.IndexOf(this.Pattern, index + this.Replacement.Length);
            }

            if (didMatch)
            {
                profileRegion.Tag = this.ApplyCount;
                Profiler.RegionFinish(profileRegion);
            }
            else
            {
                profileRegion.Dispose(true);
            }

            return didMatch;
        }
    }
}
