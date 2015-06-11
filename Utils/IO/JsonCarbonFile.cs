namespace CarbonCore.Utils.IO
{
    using CarbonCore.Utils.Compat.IO;
    using CarbonCore.Utils.Json;

    using Newtonsoft.Json;

    [JsonConverter(typeof(JsonCarbonFileConverter))]
    public class JsonCarbonFile : CarbonFile
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JsonCarbonFile(string path)
            : base(path)
        {
        }
    }
}
