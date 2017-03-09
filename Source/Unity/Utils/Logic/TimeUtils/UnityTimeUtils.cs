namespace CarbonCore.Unity.Utils.Logic.TimeUtils
{
    using System;
    using System.Runtime.InteropServices;

    using NLog;

    public static class UnityTimeUtils
    {
        public static readonly DateTime UnixBaseTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

#if WIN64
        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern int GetTimeZoneInformation(out MSTimeZoneInformation lpTimeZoneInformation);

        // This would not be neccesary but as of Unity 5.1 the TimeZoneInfo C# classes are not working properly
        public static MSTimeZoneInformation? GetCurrentTimeZone()
        {
            MSTimeZoneInformation result;
            int count = GetTimeZoneInformation(out result);
            if (count >= 1)
            {
                return result;
            }

            return null;
        }
#endif

        public static DateTime GetLocalTimeFromTimeStamp(uint timestamp)
        {
            DateTime utcTime = UnixBaseTime + TimeSpan.FromSeconds(timestamp);

#if WIN64
            MSTimeZoneInformation? timeZone = GetCurrentTimeZone();

            if (timeZone == null)
            {
                Logger.Warn("Could not get current Timezone!");
                return utcTime;
            }

            return utcTime - TimeSpan.FromMinutes(timeZone.Value.bias);
#else
            Logger.Warn("GetLocalTimeFromTimeStamp is not implemented for the current Platform, using UTCTime!");
            return utcTime;
#endif
        }
    }
}
