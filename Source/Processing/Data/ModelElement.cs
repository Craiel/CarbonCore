namespace CarbonCore.Processing.Data
{
    using System.Collections.Generic;

    public class ModelElement
    {
        public int PositionCount { get; set; }

        public int NormalCount { get; set; }

        public int TextureCount { get; set; }

        public int TangentCount { get; set; }

        public IList<float> PositionList { get; set; }

        public IList<float> NormalList { get; set; }

        public IList<float> TextureList { get; set; }

        public IList<float> TangentList { get; set; }
    }
}
