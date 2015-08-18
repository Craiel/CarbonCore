namespace CarbonCore.ContentServices.Logic.DataEntryLogic.Serializers
{
    using System.IO;

    public class BooleanSerializer : DataEntryElementSerializer
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override int MinSize
        {
            get
            {
                return 1;
            }
        }

        public override int Serialize(Stream target, object source)
        {
            bool typed = (bool)source;
            target.WriteByte(typed ? (byte)1 : (byte)0);
            return this.MinSize;
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
