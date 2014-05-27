namespace CarbonCore.JSharpBridge.Core
{
    public class DecimalFormat
    {
        private readonly string format;

        public DecimalFormat(string format)
        {
            this.format = format;
        }

        public string Format(double value)
        {
            throw new System.NotImplementedException();
        }
    }
}
