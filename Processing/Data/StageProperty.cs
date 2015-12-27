namespace CarbonCore.Processing.Data
{
    public enum StagePropertyType
    {
        String,
        Float,
        Int
    }

    public class StageProperty
    {
        public string ToStringUtf8()
        {
            throw new System.NotImplementedException();
        }

        public byte[] ToByteArray()
        {
            throw new System.NotImplementedException();
        }

        public StagePropertyType Type { get; set; }
    }
}
