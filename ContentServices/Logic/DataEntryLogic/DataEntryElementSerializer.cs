namespace CarbonCore.ContentServices.Logic.DataEntryLogic
{
    public abstract class DataEntryElementSerializer
    {
        public abstract int EstimatedSize { get; }

        public abstract byte[] Serialize();

        public abstract object Deserialize();
    }
}
