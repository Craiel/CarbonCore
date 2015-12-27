namespace CarbonCore.Processing.Data
{
    using System.Collections.Generic;

    public class Model
    {
        public int Version { get; set; }

        public string Name { get; set; }

        public bool TangentsCalculated { get; set; }

        public int ElementsCount { get; set; }

        public IList<ModelElement> ElementsList { get; set; }

        public int MaterialsCount { get; set; }

        public IList<ModelMaterial> MaterialsList { get; set; }

        public IList<uint> IndicesList { get; set; }
    }
}
