namespace CarbonCore.ContentServices.Data.Tiled
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class TiledMapTileset
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [JsonProperty("firstgid")]
        public ushort FirstGID { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("imagewidth")]
        public ushort ImageWidth { get; set; }

        [JsonProperty("imageheight")]
        public ushort ImageHeight { get; set; }

        [JsonProperty("margin")]
        public ushort Margin { get; set; }

        [JsonProperty("spacing")]
        public ushort Spacing { get; set; }

        [JsonProperty("tilecount")]
        public ushort TileCount { get; set; }

        [JsonProperty("tilewidth")]
        public ushort TileWidth { get; set; }

        [JsonProperty("tileheight")]
        public ushort TileHeight { get; set; }
    }
}
