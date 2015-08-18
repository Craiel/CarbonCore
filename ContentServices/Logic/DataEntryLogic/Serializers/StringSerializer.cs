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

        public override int MinSize
        {
            get
            {
                return 3;
            }
        }

        public override int Serialize(Stream target, object value)
        {
            string typed = (string)value;
            if (string.IsNullOrEmpty(typed))
            {
                target.WriteByte(0);
                return 1;
            }

            target.WriteByte(1);
            byte[] data = Encoding.UTF8.GetBytes(typed);

            byte[] length = BitConverter.GetBytes((Int16)data.Length);
            target.Write(length, 0, length.Length);

            target.Write(data, 0, data.Length);

            // Return the overall written size
            return 3 + data.Length;
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
                return default(string);
            }

            byte[] length = new byte[2];
            source.Read(length, 0, length.Length);

            byte[] data = new byte[BitConverter.ToInt16(length, 0)];
            source.Read(data, 0, data.Length);

            return Encoding.UTF8.GetString(data);
        }
    }
}
