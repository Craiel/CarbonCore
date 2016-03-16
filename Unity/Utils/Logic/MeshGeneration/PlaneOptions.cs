namespace CarbonCore.Unity.Utils.Logic.MeshGeneration
{
    using CarbonCore.Unity.Utils.Logic.Enums;
    
    public class PlaneOptions
    {
        public static readonly PlaneOptions Default = new PlaneOptions();

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public PlaneOptions()
        {
            this.Name = "New Plane";
            this.Orientation = Orientation.Horizontal;
            this.Width = 1.0f;
            this.Height = 1.0f;

            this.WidthSegments = 2;
            this.HeightSegments = 2;

            this.TwoSided = false;

            this.Anchor = AnchorPoint.TopLeft;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public Orientation Orientation { get; set; }

        public float Width { get; set; }
        public float Height { get; set; }

        public int WidthSegments { get; set; }
        public int HeightSegments { get; set; }

        public bool TwoSided { get; set; }

        public AnchorPoint Anchor { get; set; }
    }
}
