namespace CarbonCore.Processing.Resource.Stage
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using CarbonCore.Processing.Data;
    
    public abstract class StagePropertyElement
    {
        public string Id { get; set; }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class StagePropertyElementString : StagePropertyElement
    {
        public StagePropertyElementString()
        {
        }

        public StagePropertyElementString(StageProperty source)
            : this()
        {
            this.Value = source.ToStringUtf8();
        }

        public string Value { get; set; }

        /*public override StageProperty.Builder GetBuilder()
        {
            StageProperty.Builder builder = base.GetBuilder();
            builder.Data = ByteString.CopyFrom(this.Value, Encoding.UTF8);
            builder.Type = Protocol.Resource.StageProperty.Types.StagePropertyType.String;
            return builder;
        }*/
    }

    public class StagePropertyElementFloat : StagePropertyElement
    {
        public StagePropertyElementFloat()
        {
        }

        public StagePropertyElementFloat(StageProperty source)
            : this()
        {
            this.Value = BitConverter.ToSingle(source.ToByteArray(), 0);
        }

        public float Value { get; set; }

        /*public override Protocol.Resource.StageProperty.Builder GetBuilder()
        {
            Protocol.Resource.StageProperty.Builder builder = base.GetBuilder();
            builder.Data = ByteString.CopyFrom(BitConverter.GetBytes(this.Value));
            builder.Type = Protocol.Resource.StageProperty.Types.StagePropertyType.Float;
            return builder;
        }*/
    }

    public class StagePropertyElementInt : StagePropertyElement
    {
        public StagePropertyElementInt()
        {
        }

        public StagePropertyElementInt(StageProperty source)
            : this()
        {
            this.Value = BitConverter.ToInt32(source.ToByteArray(), 0);
        }

        public int Value { get; set; }

        /*public override Protocol.Resource.StageProperty.Builder GetBuilder()
        {
            Protocol.Resource.StageProperty.Builder builder = base.GetBuilder();
            builder.Data = ByteString.CopyFrom(BitConverter.GetBytes(this.Value));
            builder.Type = Protocol.Resource.StageProperty.Types.StagePropertyType.Int;
            return builder;
        }*/
    }
}
