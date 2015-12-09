namespace CarbonCore.ContentServices.Compat.Logic.DataEntryLogic
{
    using System.IO;

    public abstract class DataEntryElementSerializer
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract void SerializeImplicit(Stream target, object value);

        public abstract object DeserializeImplicit(Stream source);
    }
}
