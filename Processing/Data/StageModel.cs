namespace CarbonCore.Processing.Data
{
    using System.Collections.Generic;

    public class StageModel
    {
        public string Id { get; set; }

        public int TranslationCount { get; set; }

        public int RotationCount { get; set; }

        public int ScaleCount { get; set; }

        public bool HasReferenceId { get; set; }

        public int? ReferenceId { get; set; }

        public IList<float> TranslationList { get; set; }

        public IList<float> RotationList { get; set; }

        public IList<float> ScaleList { get; set; }

        public bool HasLayerFlags { get; set; }

        public int LayerFlags { get; set; }

        public int PropertiesCount { get; set; }

        public IList<StageProperty> PropertiesList { get; set; }

        public int ChildrenCount { get; set; }

        public IList<StageModel> ChildrenList { get; set; }
    }
}
