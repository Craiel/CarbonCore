namespace CarbonCore.Utils.Compat.Network
{
    using CarbonCore.Utils.Compat.Contracts.Network;

    public enum TcpDataState
    {
        Connected,
        TimedOut,
        Disconnected,
    }

    public class CoreTcpData
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CoreTcpData()
        {
            this.State = TcpDataState.Connected;
        }

        public CoreTcpData(int bufferSize)
            : this()
        {
            this.Data = new byte[bufferSize];
        }

        public CoreTcpData(byte[] data)
            : this()
        {
            this.Data = data;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public ICoreTcpClient Client { get; set; }

        public TcpDataState State { get; set; }

        public byte[] Data { get; set; }

        public int BytesRead { get; set; }
        public int BytesWritten { get; set; }
    }
}
