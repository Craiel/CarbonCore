namespace CarbonCore.Processing.Resource.Stage
{
    using CarbonCore.UtilsDX;

    using SharpDX;

    public class StageCameraElement : StageElement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StageCameraElement()
        {
        }

        public StageCameraElement(Protocol.Resource.StageCamera data)
            : this()
        {
            System.Diagnostics.Debug.Assert(data.PositionCount == 3, "Position data has invalid count");
            System.Diagnostics.Debug.Assert(data.RotationCount == 4, "Rotation data has invalid count");

            this.Id = data.Id;

            this.Position = VectorExtension.Vector3FromList(data.PositionList);
            this.Rotation = new Quaternion(VectorExtension.Vector4FromList(data.RotationList));

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
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }

        public float FieldOfView { get; set; }

        public Protocol.Resource.StageCamera.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.StageCamera.Builder { Id = this.Id, FieldOfView = this.FieldOfView };

            builder.AddRangePosition(this.Position.ToList());
            builder.AddRangeRotation(this.Rotation.ToList());

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
