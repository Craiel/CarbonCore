namespace CarbonCore.JSharpBridge.Core
{
    using System;

    public class DateFormat
    {
        private readonly string formatString;

        public DateFormat(string format)
        {
            this.formatString = format;
        }

        public string Format(long date)
        {
            throw new NotImplementedException();
        }

        public string Format(Date target)
        {
            return target.ToString(this.formatString);
        }

        public static Date GetDateTimeInstance(int i, int i1)
        {
            throw new System.NotImplementedException();
        }

        public static Date GetDateTimeInstance()
        {
            throw new System.NotImplementedException();
        }
    }
}
