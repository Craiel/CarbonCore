namespace CarbonCore.Utils.Contracts.Network
{
    public interface ITcpClient
    {
        bool IsConnected { get; }

        void Connect(string address);
        void Disconnect();
    }
}
