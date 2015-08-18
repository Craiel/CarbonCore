namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using CarbonCore.ContentServices.Logic.DataEntryLogic;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class DoubleSerializer : DataEntryElementSerializer
    {
        private static DoubleSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static DoubleSerializer Instance
        {
            get
            {
                return instance ?? (instance = new DoubleSerializer());
            }
        }

        public override int MinSize
        {
            get
            {
                return 9;
            }
        }

        public override int Serialize(Stream target, object source)
        {
            Double typed = (Double)source;
            if (Math.Abs(typed - default(Double)) < double.Epsilon)
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(typed);

            target.Write(data, 0, 8);
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
                return default(double);
            }

            byte[] data = new byte[8];
            source.Read(data, 0, 8);

            return BitConverter.ToDouble(data, 0);
        }
    }
}
