namespace CarbonCore.Processing.Resource.Model
{
    using CarbonCore.Processing.Data;
    using CarbonCore.Utils.Edge.DirectX;

    using SharpDX;

    public class ModelMaterialElement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelMaterialElement()
        {
        }

        public ModelMaterialElement(ModelMaterial data)
            : this()
        {
            this.Type = data.Type;

            this.Name = data.Name;
            this.DiffuseTexture = data.DiffuseTexture;
            this.NormalTexture = data.NormalTexture;
            this.AlphaTexture = data.AlphaTexture;
            this.SpecularTexture = data.SpecularTexture;

            this.Shinyness = data.Shinyness;
            this.Refraction = data.Refraction;

            if (data.HasTransparancy)
            {
                this.Transparency = data.Transparancy;
            }

            if (data.ColorDiffuseCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.ColorDiffuseCount == 4, "Color data has invalid count");
                this.ColorDiffuse = VectorExtension.Vector4FromList(data.ColorDiffuseList);
            }

            if (data.ColorSpecularCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.ColorSpecularCount == 4, "Color data has invalid count");
                this.ColorSpecular = VectorExtension.Vector4FromList(data.ColorSpecularList);
            }

            if (data.ColorEmissionCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.ColorEmissionCount == 4, "Color data has invalid count");
                this.ColorEmission = VectorExtension.Vector4FromList(data.ColorEmissionList);
            }

            if (data.ColorAmbientCount > 0)
            {
                System.Diagnostics.Debug.Assert(data.ColorAmbientCount == 4, "Color data has invalid count");
                this.ColorAmbient = VectorExtension.Vector4FromList(data.ColorAmbientList);
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ModelMaterialType Type { get; set; }

        public string Name { get; set; }
        public string DiffuseTexture { get; set; }
        public string NormalTexture { get; set; }
        public string AlphaTexture { get; set; }
        public string SpecularTexture { get; set; }

        public float Shinyness { get; set; }
        public float Refraction { get; set; }
        public float? Transparency { get; set; }

        public Vector4? ColorDiffuse { get; set; }
        public Vector4? ColorSpecular { get; set; }
        public Vector4? ColorEmission { get; set; }
        public Vector4? ColorAmbient { get; set; }

        /*public ModelMaterial.Builder GetBuilder()
        {
            var builder = new ModelMaterial.Builder
                              {
                                  Type = this.Type,
                                  Name = this.Name,
                                  Shinyness = this.Shinyness,
                                  Refraction = this.Refraction
                              };

            if (this.Transparency != null)
            {
                builder.SetTransparancy((float)this.Transparency);
            }

            if (!string.IsNullOrEmpty(this.DiffuseTexture))
            {
                builder.DiffuseTexture = this.DiffuseTexture;
            }

            if (!string.IsNullOrEmpty(this.NormalTexture))
            {
                builder.NormalTexture = this.NormalTexture;
            }

            if (!string.IsNullOrEmpty(this.AlphaTexture))
            {
                builder.AlphaTexture = this.AlphaTexture;
            }

            if (!string.IsNullOrEmpty(this.SpecularTexture))
            {
                builder.SpecularTexture = this.SpecularTexture;
            }

            if (this.ColorDiffuse != null)
            {
                builder.AddRangeColorDiffuse(this.ColorDiffuse.Value.ToList());
            }

            if (this.ColorSpecular != null)
            {
                builder.AddRangeColorSpecular(this.ColorSpecular.Value.ToList());
            }

            if (this.ColorEmission != null)
            {
                builder.AddRangeColorEmission(this.ColorEmission.Value.ToList());
            }

            if (this.ColorAmbient != null)
            {
                builder.AddRangeColorAmbient(this.ColorAmbient.Value.ToList());
            }

            return builder;
        }*/

        public ModelMaterialElement Clone()
        {
            return new ModelMaterialElement
            {
                Name = this.Name,
                DiffuseTexture = this.DiffuseTexture,
                NormalTexture = this.NormalTexture,
                AlphaTexture = this.AlphaTexture,
                SpecularTexture = this.SpecularTexture,
                Transparency = this.Transparency,
                Shinyness = this.Shinyness,
                Refraction = this.Refraction,
                ColorDiffuse = this.ColorDiffuse,
                ColorSpecular = this.ColorSpecular,
                ColorAmbient = this.ColorAmbient,
                ColorEmission = this.ColorEmission
            };
        }
    }
}