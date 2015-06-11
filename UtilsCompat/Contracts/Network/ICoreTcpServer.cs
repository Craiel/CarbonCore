namespace CarbonCore.Utils.Compat.Contracts.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    
    public delegate void TcpServerEventHandler(ICoreTcpClient client);

    public interface ICoreTcpServer
    {
        event Action ServerStarted;
        event Action ServerStopped;

        event TcpServerEventHandler ClientConnected;
        event TcpServerEventHandler ClientDisconnected;

        int? Port { get; set; }

        long? ClientTimeout { get; set; }

        bool IsRunning { get; }

        IPEndPoint GetEndpoint();

        void Start();
        void Stop();

        void Disconnect(TcpClient client);
    }
}
