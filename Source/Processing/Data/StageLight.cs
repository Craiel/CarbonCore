namespace CarbonCore.Processing.Data
{
    using System.Collections.Generic;

    public enum StageLightType
    {
        
    }

    public class StageLight
    {
        public string Id { get; set; }

        public StageLightType Type { get; set; }

        public int LocationCount { get; set; }

        public IList<float> LocationList { get; set; }

        public int DirectionCount { get; set; }

        public IList<float> DirectionList { get; set; }

        public int ColorCount { get; set; }

        public IList<float> ColorList { get; set; }

        public float Radius { get; set; }

        public float Intensity { get; set; }

        public float AmbientIntensity { get; set; }

        public float SpotSize { get; set; }

        public float Angle { get; set; }

        public bool HasLayerFlags { get; set; }

        public int LayerFlags { get; set; }

        public int PropertiesCount { get; set; }

        public IList<StageProperty> PropertiesList { get; set; }
    }
}
