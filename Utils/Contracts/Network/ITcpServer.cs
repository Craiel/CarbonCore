namespace CarbonCore.Utils.Contracts.Network
{
    using System;
    using System.Net.Sockets;

    public delegate void TcpServerEventHandler(TcpClient client);

    public interface ITcpServer : IDisposable
    {
        event Action OnServerStarted;
        event Action OnServerStopped;

        event TcpServerEventHandler OnClientConnected;
        event TcpServerEventHandler OnClientDisconnected;

        int Port { get; set; }

        bool IsRunning { get; }

        void Start();
        void Stop();

        void Disconnect(TcpClient client);
    }
}
