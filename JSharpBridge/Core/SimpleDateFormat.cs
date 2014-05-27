namespace CarbonCore.JSharpBridge.Core
{
    public class SimpleDateFormat : DateFormat
    {
        public SimpleDateFormat(string format = null)
            : base(format)
        {
        }

        public Date Parse(string trim)
        {
            throw new System.NotImplementedException();
        }
    }
}
