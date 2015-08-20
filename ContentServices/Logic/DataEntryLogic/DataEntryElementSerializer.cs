namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    using System.IO;

    public abstract class DataEntryElementSerializer
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract long MinSize { get; }

        public abstract long Serialize(Stream target, object value);

        public abstract object Deserialize(Stream source);
    }
}
