namespace CarbonCore.Processing.Data
{
    using System.Collections.Generic;

    public class ModelGroup
    {
        public int Version { get; set; }

        public string Name { get; set; }

        public int OffsetCount { get; set; }

        public int ScaleCount { get; set; }

        public int RotationCount { get; set; }

        public IList<float> OffsetList { get; set; }

        public IList<float> ScaleList { get; set; }

        public IList<float> RotationList { get; set; }

        public int ModelsCount { get; set; }

        public IEnumerable<Model> ModelsList { get; set; }

        public int GroupsCount { get; set; }

        public IEnumerable<ModelGroup> GroupsList { get; set; }

        public int TransformationsCount { get; set; }

        public IEnumerable<StoredMatrix> TransformationsList { get; set; }
    }
}
