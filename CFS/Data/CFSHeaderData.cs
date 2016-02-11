namespace CarbonCore.CFS.Data
{
    public class CFSHeaderData
    {
        public byte[] Head { get; set; }

        public uint Version { get; set; }

        public uint Reserved { get; set; }

        public ulong MainTableAddress { get; set; }

        public ulong BackupTableAddress { get; set; }

        public byte[] Padding { get; set; }
    }
}
