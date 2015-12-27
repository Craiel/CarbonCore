namespace CarbonCore.Processing.Data
{
    using System.Collections.Generic;

    public class StageCamera
    {
        public int PositionCount { get; set; }

        public int RotationCount { get; set; }

        public IList<float> PositionList { get; set; }

        public IList<float> RotationList { get; set; }

        public bool HasLayerFlags { get; set; }

        public int LayerFlags { get; set; }

        public int PropertiesCount { get; set; }

        public IList<StageProperty> PropertiesList { get; set; }

        public string Id { get; set; }
    }
}
