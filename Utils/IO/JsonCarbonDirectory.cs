namespace CarbonCore.Utils.IO
{
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [JsonConverter(typeof(JsonCarbonDirectoryConverter))]
    public class JsonCarbonDirectory : CarbonDirectory
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JsonCarbonDirectory(string path)
            : base(path)
        {
        }

        public JsonCarbonDirectory(CarbonFile file)
            : base(file)
        {
        }
    }
}
