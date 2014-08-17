namespace CarbonCore.Utils.IO
{
    using System.IO;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class CarbonDirectoryFilter
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonDirectoryFilter(CarbonDirectory directory, params string[] filterStrings)
        {
            this.Directory = directory;
            this.FilterStrings = filterStrings;
            this.Option = SearchOption.TopDirectoryOnly;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public CarbonDirectory Directory { get; private set; }

        public string[] FilterStrings { get; set; }

        public SearchOption Option { get; set; }
    }
}
