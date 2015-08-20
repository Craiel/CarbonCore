namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Text;

    [SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Reviewed. Suppression is OK here.")]
    public class StringSerializer : DataEntryElementSerializer
    {
        private static StringSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static StringSerializer Instance
        {
            get
            {
                return instance ?? (instance = new StringSerializer());
            }
        }

        public override long MinSize
        {
            get
            {
                return 3;
            }
        }

        public long Serialize(Stream target, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);
            byte[] data = Encoding.UTF8.GetBytes(value);

            byte[] length = BitConverter.GetBytes((Int16)data.Length);
            target.Write(length, 0, length.Length);

            target.Write(data, 0, data.Length);

            // Return the overall written size
            return 3 + data.Length;
        }

        public override long Serialize(Stream target, object value)
        {
            return this.Serialize(target, (string)value);
        }

        public override object Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == 0 || indicator == byte.MaxValue)
            {
                return null;
            }
            
            byte[] length = new byte[2];
            source.Read(length, 0, length.Length);

            byte[] data = new byte[BitConverter.ToInt16(length, 0)];
            source.Read(data, 0, data.Length);

            return Encoding.UTF8.GetString(data);
        }
    }
}
