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

        public override int MinSize
        {
            get
            {
                return 3;
            }
        }

        public override int Serialize(Stream target, object source)
        {
            byte[] typed = (byte[])source;
            if (typed == null || typed.Length <= 0)
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);

            byte[] length = BitConverter.GetBytes((Int16)typed.Length);
            target.Write(length, 0, 2);
            target.Write(typed, 0, typed.Length);

            // Return the overall written size
            return 3 + typed.Length;
        }

        public override object Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            if (indicator == 0)
            {
                return default(byte[]);
            }

            byte[] length = new byte[2];
            source.Read(length, 0, length.Length);

            byte[] data = new byte[BitConverter.ToInt16(length, 0)];
            source.Read(data, 0, data.Length);

            return data;
        }
    }
}
