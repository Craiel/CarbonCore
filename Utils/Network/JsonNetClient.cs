namespace CarbonCore.Utils.Network
{
    using CarbonCore.Utils.Contracts.Network;

    public class JsonNetClient : TcpServerClient, IJsonNetClient
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event JsonPackageHandler OnPackageReceived;

        public event JsonPackageHandler OnPackageSent;

        public void Send<T>(T package) where T : JsonNetPackage
        {
            throw new System.NotImplementedException();
        }
    }
}
