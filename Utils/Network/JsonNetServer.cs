namespace CarbonCore.Utils.Network
{
    using CarbonCore.Utils.Contracts.Network;

    public class JsonNetServer : TcpServer, IJsonNetServer
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event JsonPackageHandler OnPackageReceived;
    }
}
