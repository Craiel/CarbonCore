namespace CarbonCore.Processing.Resource.Stage
{
    using CarbonCore.UtilsDX;

    using SharpDX;

    public class StageLightElement : StageElement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StageLightElement()
        {
        }

        public StageLightElement(Protocol.Resource.StageLight data)
            : this()
        {
            this.Id = data.Id;
            this.Type = data.Type;

            if (data.LocationCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.LocationCount == 3, "Location data has invalid count");
                this.Location = VectorExtension.Vector3FromList(data.LocationList);
            }

            if (data.DirectionCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.DirectionCount == 3, "Direction data has invalid count");
                this.Direction = VectorExtension.Vector3FromList(data.DirectionList);
            }

            if (data.ColorCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.ColorCount == 3, "Color data has invalid count");
                this.Color = VectorExtension.Vector3FromList(data.ColorList);
            }

            this.Radius = data.Radius;
            this.Intensity = data.Intensity;
            this.AmbientIntensity = data.AmbientIntensity;
            this.SpotSize = data.SpotSize;
            this.Angle = data.Angle;

            if (data.HasLayerFlags)
            {
                this.LoadLayerData(data.LayerFlags);
            }

            if (data.PropertiesCount > 0)
            {
                this.LoadProperties(data.PropertiesList);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Protocol.Resource.StageLight.Types.StageLightType Type { get; set; }

        public Vector3? Location { get; set; }
        public Vector3? Direction { get; set; }
        public Vector3? Color { get; set; }

        public float Radius { get; set; }
        public float Intensity { get; set; }
        public float AmbientIntensity { get; set; }
        public float SpotSize { get; set; }
        public float Angle { get; set; }

        public Protocol.Resource.StageLight.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.StageLight.Builder
                              {
                                  Id = this.Id,
                                  Radius = this.Radius,
                                  Intensity = this.Intensity,
                                  AmbientIntensity = this.AmbientIntensity,
                                  SpotSize = this.SpotSize,
                                  Angle = this.Angle,
                                  Type = this.Type
                              };

            if (this.Location != null)
            {
                builder.AddRangeLocation(this.Location.Value.ToList());
            }

            if (this.Direction != null)
            {
                builder.AddRangeDirection(this.Direction.Value.ToList());
            }

            if (this.Color != null)
            {
                builder.AddRangeColor(this.Color.Value.ToList());
            }

            if (this.LayerFlags != null)
            {
                builder.SetLayerFlags(this.SaveLayerData());
            }

            if (this.Properties != null)
            {
                builder.AddRangeProperties(this.SaveProperties());
            }

            return builder;
        }
    }
}
