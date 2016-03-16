namespace CarbonCore.CFS.Data
{
    public class CFSFileTableData
    {
        public uint Version { get; set; }

        public ulong LastWriteTime { get; set; }

        public uint Reserved { get; set; }

        public uint Count { get; set; }

        public CFSEntryData[] Entries { get; set; }

        public byte[] Padding { get; set; }
    }
}
