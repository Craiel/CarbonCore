namespace CarbonCore.Processing.Data
{
    using System.Collections.Generic;

    public enum ModelMaterialType
    {
        Blinn
    }

    public class ModelMaterial
    {
        public string DiffuseTexture { get; set; }

        public string NormalTexture { get; set; }

        public string AlphaTexture { get; set; }

        public string SpecularTexture { get; set; }

        public float Shinyness { get; set; }

        public float Refraction { get; set; }

        public bool HasTransparancy { get; set; }

        public float? Transparancy { get; set; }

        public int ColorDiffuseCount { get; set; }

        public IList<float> ColorDiffuseList { get; set; }

        public int ColorSpecularCount { get; set; }

        public IList<float> ColorSpecularList { get; set; }

        public int ColorEmissionCount { get; set; }

        public IList<float> ColorEmissionList { get; set; }

        public int ColorAmbientCount { get; set; }

        public IList<float> ColorAmbientList { get; set; }

        public string Name { get; set; }

        public ModelMaterialType Type { get; set; }
    }
}
