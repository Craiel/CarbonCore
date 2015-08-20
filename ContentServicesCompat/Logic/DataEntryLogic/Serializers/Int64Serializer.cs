namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class Int64Serializer : DataEntryElementSerializer
    {
        private static Int64Serializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Int64Serializer Instance
        {
            get
            {
                return instance ?? (instance = new Int64Serializer());
            }
        }

        public void Serialize(Stream target, Int64? source)
        {
            if (source == null)
            {
                target.WriteByte(byte.MaxValue);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public void Serialize(Stream target, Int64 source)
        {
            if (source == default(Int64))
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(source);

            target.Write(data, 0, data.Length);
        }

        public override void Serialize(Stream target, object source)
        {
            this.Serialize(target, (Int64)source);
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
                return default(Int64);
            }

            byte[] data = new byte[8];
            source.Read(data, 0, 8);

            return BitConverter.ToInt64(data, 0);
        }
    }
}
