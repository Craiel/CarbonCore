namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic.Serializers
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
        
        public void Serialize(Stream target, bool? value)
        {
            if (value == null)
            {
                target.WriteByte(Constants.SerializationNull);
                return;
            }

            this.Serialize(target, value.Value);
        }

        public void Serialize(Stream target, bool value)
        {
            target.WriteByte(value ? (byte)1 : (byte)0);
        }

        public override void Serialize(Stream target, object source)
        {
            this.Serialize(target, (bool)source);
        }

        public override object Deserialize(Stream source)
        {
            byte indicator = (byte)source.ReadByte();
            if (indicator == Constants.SerializationNull)
            {
                return null;
            }

            return indicator == 1;
        }
    }
}
