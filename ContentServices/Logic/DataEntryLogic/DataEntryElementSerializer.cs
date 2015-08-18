namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.IO;

    public abstract class DataEntryElementSerializer
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract int MinSize { get; }

        public abstract int Serialize(Stream target, object value);

        public abstract object Deserialize(Stream source);
    }
}
