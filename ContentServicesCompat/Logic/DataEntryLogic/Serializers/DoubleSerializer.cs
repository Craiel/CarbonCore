namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    
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

        public void Serialize(Stream target, Double? source)
        {
            if (source == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            this.Serialize(target, source.Value);
        }

        public void Serialize(Stream target, Double source)
        {
            if (Math.Abs(source - default(Double)) < double.Epsilon)
            {
                target.WriteByte(0);
                return;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(source);

            target.Write(data, 0, 8);
        }

        public override void Serialize(Stream target, object source)
        {
            this.Serialize(target, (Double)source);
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
                return default(double);
            }

            byte[] data = new byte[8];
            source.Read(data, 0, 8);

            return BitConverter.ToDouble(data, 0);
        }
    }
}
