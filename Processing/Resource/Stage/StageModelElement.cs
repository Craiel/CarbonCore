namespace CarbonCore.Processing.Resource.Stage
{
    using System.Collections.Generic;

    using CarbonCore.Protocol.Resource;
    using CarbonCore.UtilsDX;

    using SharpDX;

    public class StageModelElement : StageElement
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public StageModelElement()
        {
        }

        public StageModelElement(StageModel data)
            : this()
        {
            System.Diagnostics.Debug.Assert(data.TranslationCount == 3, "Translation data has invalid count");
            System.Diagnostics.Debug.Assert(data.RotationCount == 4, "Rotation data has invalid count");
            System.Diagnostics.Debug.Assert(data.ScaleCount == 3, "Scale data has invalid count");

            this.Id = data.Id;

            if (data.HasReferenceId)
            { 
                this.ReferenceId = data.ReferenceId;
            }

            this.Translation = VectorExtension.Vector3FromList(data.TranslationList);
            this.Rotation = new Quaternion(VectorExtension.Vector4FromList(data.RotationList));
            this.Scale = VectorExtension.Vector3FromList(data.ScaleList);

            if (data.HasLayerFlags)
            {
                this.LoadLayerData(data.LayerFlags);
            }

            if (data.PropertiesCount > 0)
            {
                this.LoadProperties(data.PropertiesList);
            }

            if (data.ChildrenCount > 0)
            {
                this.Children = new StageModelElement[data.ChildrenCount];
                for (int i = 0; i < data.ChildrenList.Count; i++)
                {
                    this.Children[i] = new StageModelElement(data.ChildrenList[i]);
                }
            }
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int? ReferenceId { get; set; }

        public Vector3 Translation { get; set; }
        public Vector3 Scale { get; set; }

        public Quaternion Rotation { get; set; }

        public IList<StageModelElement> Children { get; set; }

        public StageModel.Builder GetBuilder()
        {
            var builder = new StageModel.Builder { Id = this.Id };

            if (this.ReferenceId != null)
            {
                builder.ReferenceId = (int)this.ReferenceId;
            }

            builder.AddRangeTranslation(this.Translation.ToList());
            builder.AddRangeRotation(this.Rotation.ToList());
            builder.AddRangeScale(this.Scale.ToList());

            if (this.LayerFlags != null)
            {
                builder.SetLayerFlags(this.SaveLayerData());
            }

            if (this.Properties != null)
            {
                builder.AddRangeProperties(this.SaveProperties());
            }

            if (this.Children != null)
            {
                foreach (StageModelElement child in this.Children)
                {
                    builder.AddChildren(child.GetBuilder().Build());
                }
            }

            return builder;
        }
    }
}
