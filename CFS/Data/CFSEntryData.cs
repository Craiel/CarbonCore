namespace CarbonCore.CFS.Data
{
    public class CFSEntryData
    {
        public bool IsCompressed { get; set; }

        public byte Chunks { get; set; }

        public ulong[] Offsets { get; set; }
        public ulong[] Sizes { get; set; }

        public ulong LastWriteTime { get; set; }

        public uint Reserved { get; set; }

        public byte[] Padding { get; set; }
    }
}
