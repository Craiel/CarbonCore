namespace CarbonCore.JSharpBridge.Core
{
    using System;

    public class Date
    {
        private readonly DateTime dateTime;

        public Date()
        {
            this.dateTime = DateTime.Now;
        }

        public Date(long ticks)
        {
            this.dateTime = new DateTime(ticks);
        }

        public string ToString(string format)
        {
            return this.dateTime.ToString(format);
        }

        public bool Before(Date date)
        {
            return Utils.Diagnostics.Internal.NotImplemented<bool>();
        }

        public long GetTime()
        {
            return Utils.Diagnostics.Internal.NotImplemented<long>();
        }

        public string Format(Date par1Date)
        {
            return Utils.Diagnostics.Internal.NotImplemented<string>();
        }
    }
}
