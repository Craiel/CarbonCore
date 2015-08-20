namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class Int32Serializer : DataEntryElementSerializer
    {
        private static Int32Serializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static Int32Serializer Instance
        {
            get
            {
                return instance ?? (instance = new Int32Serializer());
            }
        }

        public override int MinSize
        {
            get
            {
                return 5;
            }
        }

        public int Serialize(Stream target, Int32? source)
        {
            if (source == null)
            {
                target.WriteByte(byte.MaxValue);
                return 1;
            }

            return this.Serialize(target, source.Value);
        }

        public int Serialize(Stream target, Int32 source)
        {
            if (source == default(Int32))
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(source);

            target.Write(data, 0, data.Length);
            return this.MinSize;
        }

        public override int Serialize(Stream target, object source)
        {
            return this.Serialize(target, (Int32)source);
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
                return default(Int32);
            }

            byte[] data = new byte[4];
            source.Read(data, 0, 4);

            return BitConverter.ToInt32(data, 0);
        }
    }
}
