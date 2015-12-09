namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class Int32ArraySerializer : DataEntryElementSerializer
    {
        private static Int32ArraySerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Int32ArraySerializer Instance
        {
            get
            {
                return instance ?? (instance = new Int32ArraySerializer());
            }
        }

        public void Serialize(Stream target, Int32[] source)
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

        public Int32[] Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            if (indicator == 0)
            {
                return new Int32[0];
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.Serialize(target, (Int32[])source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.Deserialize(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private Int32[] DoDeserialize(Stream source)
        {
            // Read the count
            byte[] data = new byte[4];
            source.Read(data, 0, 4);

            int count = BitConverter.ToInt32(data, 0);

            // Read the values
            Int32[] result = new Int32[count];
            for (var i = 0; i < count; i++)
            {
                source.Read(data, 0, 4);
                result[i] = BitConverter.ToInt32(data, 0);
            }

            return result;
        }
    }
}
