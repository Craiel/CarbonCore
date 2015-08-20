namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
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

        public override long MinSize
        {
            get
            {
                return 3;
            }
        }

        public long Serialize(Stream target, byte[] source)
        {
            if (source == null || source.Length <= 0)
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);

            byte[] length = BitConverter.GetBytes((Int16)source.Length);
            target.Write(length, 0, 2);
            target.Write(source, 0, source.Length);

            // Return the overall written size
            return 3 + source.Length;
        }

        public override long Serialize(Stream target, object source)
        {
            return this.Serialize(target, (byte[])source);
        }

        public override object Deserialize(Stream source)
        {
            if ((byte)source.ReadByte() == 0)
            {
                return null;
            }

            byte[] length = new byte[2];
            source.Read(length, 0, length.Length);

            byte[] data = new byte[BitConverter.ToInt16(length, 0)];
            source.Read(data, 0, data.Length);

            return data;
        }
    }
}
