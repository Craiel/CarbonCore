namespace CarbonCore.ContentServices.Compat.Data.Tiled
{
    using System;

    using CarbonCore.ContentServices.Compat.Logic.Enums;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptIn)]
    public class TiledMapData
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        [JsonProperty("version")]
        public ushort Version { get; set; }

        [JsonProperty("width")]
        public ushort Width { get; set; }

        [JsonProperty("height")]
        public ushort Height { get; set; }

        [JsonProperty("tilewidth")]
        public ushort TileWidth { get; set; }

        [JsonProperty("tileheight")]
        public ushort TileHeight { get; set; }

        [JsonProperty("orientation")]
        public string OrientationValue { get; set; }

        [JsonProperty("renderorder")]
        public string RenderOrderValue { get; set; }

        [JsonProperty("nextobjectid")]
        public ushort NextObjectId { get; set; }

        [JsonProperty("layers")]
        public TiledMapLayerData[] Layers { get; set; }
        
        [JsonProperty("tilesets")]
        public TiledMapTileset[] Tilesets { get; set; }

        public TiledMapOrientation Orientation
        {
            get
            {
                if (string.IsNullOrEmpty(this.OrientationValue))
                {
                    return TiledMapOrientation.Unknown;
                }

                if (this.OrientationValue.Equals("orthogonal", StringComparison.OrdinalIgnoreCase))
                {
                    return TiledMapOrientation.Orthogonal;
                }

                return TiledMapOrientation.Unknown;
            }
        }

        public TiledMapRenderOrder RenderOrder
        {
            get
            {
                if (string.IsNullOrEmpty(this.RenderOrderValue))
                {
                    return TiledMapRenderOrder.Unknown;
                }

                if (this.RenderOrderValue.Equals("right-down", StringComparison.OrdinalIgnoreCase))
                {
                    return TiledMapRenderOrder.RightDown;
                }

                return TiledMapRenderOrder.Unknown;
            }
        }
    }
}
