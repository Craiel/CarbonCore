namespace CarbonCore.Utils.Contracts.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    
    public delegate void TcpServerEventHandler(ICoreTcpClient client);

    public interface ICoreTcpServer
    {
        event Action OnServerStarted;
        event Action OnServerStopped;

        event TcpServerEventHandler OnClientConnected;
        event TcpServerEventHandler OnClientDisconnected;

        int? Port { get; set; }

        long? ClientTimeout { get; set; }

        bool IsRunning { get; }

        IPEndPoint GetEndpoint();

        void Start();
        void Stop();

        void Disconnect(TcpClient client);
    }
}
