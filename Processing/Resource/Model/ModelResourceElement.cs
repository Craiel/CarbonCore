namespace CarbonCore.Processing.Resource.Model
{
    using CarbonCore.Processing.Data;
    using CarbonCore.Utils.Edge.DirectX;

    using SharpDX;

    public class ModelResourceElement
    {
        internal const int Version = 1;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelResourceElement()
        {
        }

        public ModelResourceElement(ModelElement data)
            : this()
        {
            System.Diagnostics.Debug.Assert(data.PositionCount == 3, "Position data has invalid count");
            
            this.Position = VectorExtension.Vector3FromList(data.PositionList);

            if (data.NormalCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.NormalCount == 3, "Normal data has invalid count");
                this.Normal = VectorExtension.Vector3FromList(data.NormalList);
            }

            if (data.TextureCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.TextureCount == 2, "Texture data has invalid count");
                this.Texture = VectorExtension.Vector2FromList(data.TextureList);
            }

            if (data.TangentCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.TangentCount == 4, "Tangent data has invalid count");
                this.Tangent = VectorExtension.Vector4FromList(data.TangentList);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Vector3 Position { get; set; }
        public Vector3? Normal { get; set; }
        public Vector2? Texture { get; set; }
        public Vector4? Tangent { get; set; }

        /*public Protocol.Resource.ModelElement.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.ModelElement.Builder();

            builder.AddRangePosition(this.Position.ToList());

            if (this.Normal != null)
            {
                builder.AddRangeNormal(this.Normal.Value.ToList());
            }

            if (this.Texture != null)
            {
                builder.AddRangeTexture(this.Texture.Value.ToList());
            }

            if (this.Tangent != null)
            {
                builder.AddRangeTangent(this.Tangent.Value.ToList());
            }

            return builder;
        }*/
    }
}
