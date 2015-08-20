namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class FloatSerializer : DataEntryElementSerializer
    {
        private static FloatSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static FloatSerializer Instance
        {
            get
            {
                return instance ?? (instance = new FloatSerializer());
            }
        }

        public override long MinSize
        {
            get
            {
                return 5;
            }
        }

        public long Serialize(Stream target, Single? source)
        {
            if (source == null)
            {
                target.WriteByte(byte.MaxValue);
                return 1;
            }

            return this.Serialize(target, source.Value);
        }

        public long Serialize(Stream target, Single source)
        {
            if (Math.Abs(source - default(Single)) < float.Epsilon)
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);

            byte[] data = BitConverter.GetBytes(source);

            target.Write(data, 0, 4);
            return this.MinSize;
        }

        public override long Serialize(Stream target, object source)
        {
            return this.Serialize(target, (Single)source);
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
                return default(float);
            }

            byte[] data = new byte[4];
            source.Read(data, 0, 4);

            return BitConverter.ToSingle(data, 0);
        }
    }
}
