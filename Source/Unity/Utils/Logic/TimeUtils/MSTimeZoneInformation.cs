namespace CarbonCore.Unity.Utils.Logic.TimeUtils
{
#if WIN64
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    public struct MSTimeZoneInformation
    {
        public int bias;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string standardName;

        public MSSystemTime standardDate;

        public int standardBias;

        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
        public string daylightName;

        public MSSystemTime daylightDate;

        public int daylightBias;
    }
#endif
}
