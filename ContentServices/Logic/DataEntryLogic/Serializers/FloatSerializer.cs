namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class FloatSerializer : DataEntryElementSerializer
    {
        private static FloatSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static FloatSerializer Instance
        {
            get
            {
                return instance ?? (instance = new FloatSerializer());
            }
        }

        public void SerializeNullable(Stream target, float? source)
        {
            if (source == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public void Serialize(Stream target, float source)
        {
            if (Math.Abs(source - default(float)) < float.Epsilon)
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(source);

            target.Write(data, 0, 4);
        }

        public float Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                throw new InvalidDataException();
            }

            if (indicator == 0)
            {
                return default(float);
            }

            return this.DoDeserialize(source);
        }

        public float? DeserializeNullable(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(float);
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.SerializeNullable(target, (float)source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.DeserializeNullable(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private float DoDeserialize(Stream source)
        {
            byte[] data = new byte[4];
            source.Read(data, 0, 4);

            return BitConverter.ToSingle(data, 0);
        }
    }
}
