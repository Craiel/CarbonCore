namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class ByteArraySerializer : DataEntryElementSerializer
    {
        private static ByteArraySerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static ByteArraySerializer Instance
        {
            get
            {
                return instance ?? (instance = new ByteArraySerializer());
            }
        }

        public void Serialize(Stream target, byte[] source)
        {
            if (source == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            if (source.Length <= 0)
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            byte[] length = BitConverter.GetBytes((Int16)source.Length);
            target.Write(length, 0, 2);
            target.Write(source, 0, source.Length);
        }

        public override void Serialize(Stream target, object source)
        {
            this.Serialize(target, (byte[])source);
        }

        public override object Deserialize(Stream source)
        {
            int indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            if (indicator == 0)
            {
                return new byte[0];
            }

            byte[] length = new byte[2];
            source.Read(length, 0, length.Length);

            byte[] data = new byte[BitConverter.ToInt16(length, 0)];
            source.Read(data, 0, data.Length);

            return data;
        }
    }
}
