namespace CarbonCore.Unity.Utils.Logic.TimeUtils
{
#if WIN64
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct MSSystemTime
    {
        public short year;
        public short month;
        public short dayOfWeek;
        public short day;
        public short hour;
        public short minute;
        public short second;
        public short milliseconds;
    }
#endif
}
