namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class FloatArraySerializer : DataEntryElementSerializer
    {
        private static FloatArraySerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static FloatArraySerializer Instance
        {
            get
            {
                return instance ?? (instance = new FloatArraySerializer());
            }
        }

        public void Serialize(Stream target, float[] source)
        {
            if (source == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            if (source.Length == 0)
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            // Count
            byte[] data = BitConverter.GetBytes(source.Length);
            target.Write(data, 0, data.Length);

            // Entries
            for (var i = 0; i < source.Length; i++)
            {
                data = BitConverter.GetBytes(source[i]);
                target.Write(data, 0, data.Length);
            }
        }
        
        public float[] Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            if (indicator == 0)
            {
                return new float[0];
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.Serialize(target, (float[])source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.Deserialize(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private float[] DoDeserialize(Stream source)
        {
            // Read the count
            byte[] data = new byte[4];
            source.Read(data, 0, 4);

            int count = BitConverter.ToInt32(data, 0);

            // Read the values
            float[] result = new float[count];
            for (var i = 0; i < count; i++)
            {
                source.Read(data, 0, 4);
                result[i] = BitConverter.ToSingle(data, 0);
            }

            return result;
        }
    }
}
