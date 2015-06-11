namespace CarbonCore.Utils.Compat.Contracts.Network
{
    using System.Net;
    using System.Net.Sockets;

    public delegate void TcpClientEventHandler(ICoreTcpClient client);

    public interface ICoreTcpClient
    {
        event TcpClientEventHandler OnConnected;
        event TcpClientEventHandler OnDisconnected;

        long BytesSent { get; }

        bool IsConnected { get; }

        IPEndPoint EndPoint { get; }

        void Connect(IPEndPoint endPoint);
        void Disconnect();

        NetworkStream GetStream();

        void Close();

        void SetClient(TcpClient client);

        void Send(byte[] data);
        void Receive();
    }
}
