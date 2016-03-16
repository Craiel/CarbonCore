namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class BooleanArraySerializer : DataEntryElementSerializer
    {
        private static BooleanArraySerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static BooleanArraySerializer Instance
        {
            get
            {
                return instance ?? (instance = new BooleanArraySerializer());
            }
        }

        public void Serialize(Stream target, bool[] source)
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

        public bool[] Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            if (indicator == 0)
            {
                return new bool[0];
            }

            return this.DoDeserialize(source);
        }

        public override void SerializeImplicit(Stream target, object source)
        {
            this.Serialize(target, (bool[])source);
        }

        public override object DeserializeImplicit(Stream source)
        {
            return this.Deserialize(source);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private bool[] DoDeserialize(Stream source)
        {
            // Read the count
            byte[] data = new byte[4];
            source.Read(data, 0, 4);

            int count = BitConverter.ToInt32(data, 0);

            // Read the values
            bool[] result = new bool[count];
            for (var i = 0; i < count; i++)
            {
                source.Read(data, 0, 1);
                result[i] = BitConverter.ToBoolean(data, 0);
            }

            return result;
        }
    }
}
