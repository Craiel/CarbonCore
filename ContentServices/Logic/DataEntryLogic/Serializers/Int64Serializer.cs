namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
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

        public override int MinSize
        {
            get
            {
                return 10;
            }
        }

        public override int Serialize(Stream target, object source)
        {
            Int64 typed = (Int64)source;
            if (typed == default(Int64))
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(typed);

            target.Write(data, 0, data.Length);
            return this.MinSize;
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
