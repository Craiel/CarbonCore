namespace CarbonCore.CrystalBuild.Config
{
    using System.Collections.Generic;

    public class YamlProjectSource
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public YamlProjectSource()
        {
            this.IncludeFilters = new List<string>();
            this.ExcludeFilters = new List<string>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Path { get; set; }

        public IList<string> IncludeFilters { get; set; }

        public IList<string> ExcludeFilters { get; set; }

        public bool Recursive { get; set; }
    }
}
