namespace CarbonCore.Processing.Resource.Model
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using CarbonCore.Processing.Data;
    using CarbonCore.Processing.Resource;
    using CarbonCore.Utils.Edge.DirectX;

    using SharpDX;

    public class ModelResourceGroup : ProtocolResource
    {
        private const int Version = 1;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ModelResourceGroup()
        {
            this.Scale = new Vector3(1);
        }

        public ModelResourceGroup(ModelGroup data)
            : this()
        {
            this.DoLoad(data);
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Name { get; set; }
        
        public Vector3 Offset { get; set; }
        public Vector3 Scale { get; set; }
        public Quaternion Rotation { get; set; }

        public IList<ModelResource> Models { get; set; }
        public IList<ModelResourceGroup> Groups { get; set; }
        public IList<Matrix> Transformations { get; set; }

        public override void Load(Stream source)
        {
            // TODO
            /*ModelGroup entry = ModelGroup.ParseFrom(source);
            this.DoLoad(entry);*/
        }

        public override long Save(Stream target)
        {
            /*ModelGroup.Builder builder = this.GetBuilder();
            ModelGroup entry = builder.Build();
            entry.WriteTo(target);
            return entry.SerializedSize;*/

            // TODO
            return 0;
        }

        /*public ModelGroup.Builder GetBuilder()
        {
            var builder = new ModelGroup.Builder
                              {
                                  Name = this.Name,
                                  Version = Version
                              };

            builder.AddRangeOffset(this.Offset.ToList());
            builder.AddRangeScale(this.Scale.ToList());
            builder.AddRangeRotation(this.Rotation.ToList());
            
            if (this.Models != null)
            {
                foreach (ModelResource element in this.Models)
                {
                    builder.AddModels(element.GetBuilder());
                }
            }

            if (this.Groups != null)
            {
                foreach (ModelResourceGroup group in this.Groups)
                {
                    builder.AddGroups(group.GetBuilder());
                }
            }

            if (this.Transformations != null)
            {
                var matrixBuilder = new StoredMatrix.Builder();
                foreach (Matrix transformation in this.Transformations)
                {
                    matrixBuilder.Clear();
                    matrixBuilder.AddRangeData(transformation.ToArray());
                    builder.AddTransformations(matrixBuilder.Build());
                }
            }
            
            return builder;
        }*/
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void DoLoad(ModelGroup entry)
        {
            if (entry.Version != Version)
            {
                throw new InvalidOperationException("Model group version is not correct: " + entry.Version);
            }

            System.Diagnostics.Debug.Assert(entry.OffsetCount == 3, "Offset data has invalid count");
            System.Diagnostics.Debug.Assert(entry.ScaleCount == 3, "Scale data has invalid count");
            System.Diagnostics.Debug.Assert(entry.RotationCount == 4, "Rotation data has invalid count");

            this.Name = entry.Name;
            this.Offset = VectorExtension.Vector3FromList(entry.OffsetList);
            this.Scale = VectorExtension.Vector3FromList(entry.ScaleList);
            this.Rotation = QuaternionExtension.QuaterionFromList(entry.RotationList);
            
            if (entry.ModelsCount > 0)
            {
                this.Models = new List<ModelResource>(entry.ModelsCount);
                foreach (Model element in entry.ModelsList)
                {
                    this.Models.Add(new ModelResource(element));
                }
            }

            if (entry.GroupsCount > 0)
            {
                this.Groups = new List<ModelResourceGroup>(entry.GroupsCount);
                foreach (ModelGroup group in entry.GroupsList)
                {
                    this.Groups.Add(new ModelResourceGroup(group));
                }
            }

            if (entry.TransformationsCount > 0)
            {
                this.Transformations = new List<Matrix>(entry.TransformationsCount);
                foreach (StoredMatrix storedMatrix in entry.TransformationsList)
                {
                    this.Transformations.Add(new Matrix(storedMatrix.ToArray()));
                }
            }
        }
    }
}
