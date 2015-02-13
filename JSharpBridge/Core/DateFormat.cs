namespace CarbonCore.JSharpBridge.Core
{
    public class DateFormat
    {
        private readonly string formatString;

        public DateFormat(string format)
        {
            this.formatString = format;
        }

        public string Format(long date)
        {
            return Utils.Diagnostics.Internal.NotImplemented<string>();
        }

        public string Format(Date target)
        {
            return target.ToString(this.formatString);
        }

        public static Date GetDateTimeInstance(int i, int i1)
        {
            return Utils.Diagnostics.Internal.NotImplemented<Date>();
        }

        public static Date GetDateTimeInstance()
        {
            return Utils.Diagnostics.Internal.NotImplemented<Date>();
        }
    }
}
