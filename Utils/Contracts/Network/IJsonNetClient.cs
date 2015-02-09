namespace CarbonCore.Utils.Contracts.Network
{
    using CarbonCore.Utils.Network;

    public interface IJsonNetClient : ITcpClient
    {
        event JsonPackageHandler OnPackageReceived;
        event JsonPackageHandler OnPackageSent;

        void Send<T>(T package) where T : JsonNetPackage;
    }
}
