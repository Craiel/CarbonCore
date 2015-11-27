namespace CarbonCore.ContentServices.Compat.Data.Tiled
{
    using System;

    using CarbonCore.ContentServices.Compat.Logic.Enums;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class TiledMapLayerData
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("width")]
        public ushort Width { get; set; }

        [JsonProperty("height")]
        public ushort Height { get; set; }

        [JsonProperty("opacity")]
        public ushort Opacity { get; set; }

        [JsonProperty("visible")]
        public bool Visibile { get; set; }

        [JsonProperty("type")]
        public string TypeValue { get; set; }

        [JsonProperty("data")]
        public ushort[] Data { get; set; }

        [JsonProperty("objects")]
        public TiledMapObjectData[] Objects { get; set; }

        [JsonProperty("x")]
        public ushort PositionX { get; set; }

        [JsonProperty("y")]
        public ushort PositionY { get; set; }

        public TiledMapLayerType Type
        {
            get
            {
                if (string.IsNullOrEmpty(this.TypeValue))
                {
                    return TiledMapLayerType.Unknown;
                }

                if (this.TypeValue.Equals("tilelayer", StringComparison.OrdinalIgnoreCase))
                {
                    return TiledMapLayerType.TileLayer;
                }

                if (this.TypeValue.Equals("objectgroup", StringComparison.OrdinalIgnoreCase))
                {
                    return TiledMapLayerType.ObjectGroup;
                }

                return TiledMapLayerType.Unknown;
            }
        }
    }
}
