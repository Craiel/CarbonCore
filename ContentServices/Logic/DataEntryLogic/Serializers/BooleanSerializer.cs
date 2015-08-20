namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System.IO;

    public class BooleanSerializer : DataEntryElementSerializer
    {
        private static BooleanSerializer instance;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static BooleanSerializer Instance
        {
            get
            {
                return instance ?? (instance = new BooleanSerializer());
            }
        }

        public override long MinSize
        {
            get
            {
                return 1;
            }
        }

        public long Serialize(Stream target, bool? value)
        {
            if (value == null)
            {
                target.WriteByte(byte.MaxValue);
                return 1;
            }

            return this.Serialize(target, value.Value);
        }

        public long Serialize(Stream target, bool value)
        {
            target.WriteByte(value ? (byte)1 : (byte)0);
            return this.MinSize;
        }

        public override long Serialize(Stream target, object source)
        {
            return this.Serialize(target, (bool)source);
        }

        public override object Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == byte.MaxValue)
            {
                return null;
            }

            return indicator == 1;
        }
    }
}
