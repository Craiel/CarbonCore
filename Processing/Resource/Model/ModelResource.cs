namespace CarbonCore.Processing.Resource.Model
{
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Processing.Resource;
    using CarbonCore.Utils.Edge.DirectX;

    using SharpDX;

    public class ModelResource : ProtocolResource
    {
        private const int Version = 1;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelResource()
        {
        }

        public ModelResource(Protocol.Resource.Model data)
            : this()
        {
            this.DoLoad(data);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }

        public BoundingBox? BoundingBox { get; set; }
        public BoundingSphere? BoundingSphere { get; set; }

        public bool HasTangents { get; private set; }
        
        public IList<ModelResourceElement> Elements { get; set; }
        public IList<ModelMaterialElement> Materials { get; set; }

        public IList<uint> Indices { get; set; }

        public override void Load(Stream source)
        {
            Protocol.Resource.Model entry = Protocol.Resource.Model.ParseFrom(source);
            this.DoLoad(entry);
        }

        public override long Save(Stream target)
        {
            Protocol.Resource.Model.Builder builder = this.GetBuilder();
            Protocol.Resource.Model entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;
        }

        public Protocol.Resource.Model.Builder GetBuilder()
        {
            var builder = new Protocol.Resource.Model.Builder
                              {
                                  Name = this.Name,
                                  Version = Version,
                                  TangentsCalculated = this.HasTangents
                              };
            
            if (this.Elements != null)
            {
                foreach (ModelResourceElement element in this.Elements)
                {
                    builder.AddElements(element.GetBuilder());
                }
            }

            if (this.Materials != null)
            {
                foreach (ModelMaterialElement material in this.Materials)
                {
                    builder.AddMaterials(material.GetBuilder());
                }
            }

            return builder;
        }

        public void CalculateTangents()
        {
            this.HasTangents = true;

            var elements = this.Elements;
            var indices = this.Indices;
            CalculateTangents(ref elements, ref indices);
        }

        public void CalculateBoundingBox()
        {
            var points = new Vector3[this.Elements.Count];
            for (int i = 0; i < this.Elements.Count; i++)
            {
                points[i] = this.Elements[i].Position;
            }

            this.BoundingBox = SharpDX.BoundingBox.FromPoints(points);
        }

        public void CalculateBoundingSphere()
        {
            var points = new Vector3[this.Elements.Count];
            for (int i = 0; i < this.Elements.Count; i++)
            {
                points[i] = this.Elements[i].Position;
            }

            this.BoundingSphere = SharpDX.BoundingSphere.FromPoints(points);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static void CalculateTangents(ref IList<ModelResourceElement> elements, ref IList<uint> indices)
        {
            if (indices == null || indices.Count <= 0)
            {
                return;
            }

            var tan1 = new Vector3[elements.Count];
            var tan2 = new Vector3[elements.Count];
            int triangleCount = indices.Count;
            for (int i = 0; i < triangleCount; i += 3)
            {
                uint i1 = indices[i];
                uint i2 = indices[i + 1];
                uint i3 = indices[i + 2];

                ModelResourceElement e1 = elements[(int)i1];
                ModelResourceElement e2 = elements[(int)i2];
                ModelResourceElement e3 = elements[(int)i3];

                float x1 = e2.Position.X - e1.Position.X;
                float x2 = e3.Position.X - e1.Position.X;
                float y1 = e2.Position.Y - e1.Position.Y;
                float y2 = e3.Position.Y - e1.Position.Y;
                float z1 = e2.Position.Z - e1.Position.Z;
                float z2 = e3.Position.Z - e1.Position.Z;
                
                float s1 = e2.Texture.Value.X - e1.Texture.Value.X;
                float s2 = e3.Texture.Value.X - e1.Texture.Value.X;
                float t1 = e2.Texture.Value.Y - e1.Texture.Value.Y;
                float t2 = e3.Texture.Value.Y - e1.Texture.Value.Y;

                float r = 1.0f / ((s1 * t2) - (s2 * t1));

                Vector3 sdir = new Vector3(
                    ((t2 * x1) - (t1 * x2)) * r, ((t2 * y1) - (t1 * y2)) * r, ((t2 * z1) - (t1 * z2)) * r);
                Vector3 tdir = new Vector3(
                    ((s1 * x2) - (s2 * x1)) * r, ((s1 * y2) - (s2 * y1)) * r, ((s1 * z2) - (s2 * z1)) * r);

                tan1[i1] += sdir;
                tan1[i2] += sdir;
                tan1[i3] += sdir;

                tan2[i1] += tdir;
                tan2[i2] += tdir;
                tan2[i3] += tdir;
            }

            for (int i = 0; i < elements.Count; ++i)
            {
                if (elements[i].Normal == null)
                {
                    // Todo: throw
                    continue;
                }

                ModelResourceElement element = elements[i];
                Vector3 n = element.Normal.Value;
                Vector3 t = tan1[i];

                DXMathExtension.OrthoNormalize(ref n, ref t);

                float w = (Vector3.Dot(Vector3.Cross(n, t), tan2[i]) < 0.0f) ? -1.0f : 1.0f;
                elements[i].Tangent = new Vector4(tan1[i], w);
            }
        }

        private void DoLoad(Protocol.Resource.Model entry)
        {
            if (entry.Version != Version)
            {
                throw new InvalidDataException("Model version is not correct: " + entry.Version);
            }

            this.Name = entry.Name;
            this.HasTangents = entry.TangentsCalculated;

            if (entry.ElementsCount > 0)
            {
                this.Elements = new List<ModelResourceElement>(entry.ElementsCount);
                foreach (Protocol.Resource.ModelElement element in entry.ElementsList)
                {
                    this.Elements.Add(new ModelResourceElement(element));
                }
            }

            if (entry.MaterialsCount > 0)
            {
                this.Materials = new List<ModelMaterialElement>(entry.MaterialsCount);
                foreach (Protocol.Resource.ModelMaterial material in entry.MaterialsList)
                {
                    this.Materials.Add(new ModelMaterialElement(material));
                }
            }

            this.Indices = entry.IndicesList;
        }
    }
}
