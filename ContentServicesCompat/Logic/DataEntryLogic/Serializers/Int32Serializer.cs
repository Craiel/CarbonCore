namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers
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

        public void Serialize(Stream target, Int32? source)
        {
            if (source == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public void Serialize(Stream target, Int32 source)
        {
            if (source == default(Int32))
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
            this.Serialize(target, (Int32)source);
        }

        public override object Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
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
