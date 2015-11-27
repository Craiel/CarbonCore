namespace CarbonCore.ContentServices.Compat.Data.Tiled
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class TiledMapObjectData
    {
        [JsonProperty("id")]
        public ushort Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("x")]
        public ushort PositionX { get; set; }

        [JsonProperty("y")]
        public ushort PositionY { get; set; }

        [JsonProperty("width")]
        public ushort Width { get; set; }

        [JsonProperty("height")]
        public ushort Height { get; set; }

        [JsonProperty("rotation")]
        public ushort Rotation { get; set; }

        [JsonProperty("visible")]
        public bool Visible { get; set; }

        [JsonProperty("type")]
        public string TypeValue { get; set; }
    }
}
