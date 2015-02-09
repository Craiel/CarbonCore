namespace CarbonCore.Utils.Contracts.Network
{
    using CarbonCore.Utils.Network;

    public delegate void JsonPackageHandler(object sender, JsonNetPackage package);

    public interface IJsonNetServer : ITcpServer
    {
        event JsonPackageHandler OnPackageReceived;
    }
}
