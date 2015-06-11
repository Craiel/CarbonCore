namespace CarbonCore.Utils.IO
{
    using CarbonCore.Utils.Compat.IO;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class JsonCarbonDirectoryFilter : CarbonDirectoryFilter
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        [JsonConstructor]
        public JsonCarbonDirectoryFilter(CarbonDirectory directory, params string[] filterStrings)
            : base(directory, filterStrings)
        {
        }
    }
}
