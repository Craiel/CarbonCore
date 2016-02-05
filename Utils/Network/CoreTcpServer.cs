namespace CarbonCore.Utils.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Contracts.Network;
    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Threading;

    public class CoreTcpServer : ICoreTcpServer
    {
        private static readonly long DefaultClientTimeout = TimeSpan.FromSeconds(2).Ticks;

        private readonly IFactory factory;
        
        private int? port;
        
        private bool isRunning;
        private bool canAccept;

        private TcpListener listener;

        private Thread acceptThread;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CoreTcpServer(IFactory factory)
        {
            this.factory = factory;

            this.ClientTimeout = DefaultClientTimeout;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action ServerStarted;

        public event Action ServerStopped;

        public event TcpServerEventHandler ClientConnected;

        public event TcpServerEventHandler ClientDisconnected;

        public int? Port
        {
            get
            {
                return this.port;
            }

            set
            {
                if (this.IsRunning)
                {
                    throw new InvalidOperationException("Can not change port while the server is running!");
                }

                this.port = value;
            }
        }

        public long? ClientTimeout { get; set; }

        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        public IPEndPoint GetEndpoint()
        {
            if (this.port == null)
            {
                throw new InvalidOperationException("Port must be set before calling GetEndpoint()");
            }

            return new IPEndPoint(NetworkUtils.GetLocalIPAddress(), (int)this.port);
        }

        public void Start()
        {
            if (this.port == null)
            {
                throw new InvalidOperationException("Port needs to be set before Start()");
            }

            this.listener = new TcpListener(IPAddress.Any, (int)this.port);

            this.canAccept = true;
            this.acceptThread = new Thread(this.ClientAcceptMain) { IsBackground = true };
            this.acceptThread.Start();

            this.isRunning = true;

            System.Diagnostics.Debug.WriteLine("BaseTcpServer started on {0}", this.GetEndpoint().ToString());

            if (this.ServerStarted != null)
            {
                this.ServerStarted();
            }
        }
        
        public void Stop()
        {
            System.Diagnostics.Trace.Assert(this.acceptThread != null && this.acceptThread.IsAlive, "Can not stop server, was not started");
            
            this.canAccept = false;
            Thread.Sleep(10);
            this.acceptThread.Abort();
            this.acceptThread = null;

            this.listener.Stop();
            this.listener = null;

            this.isRunning = false;

            System.Diagnostics.Debug.WriteLine("BaseTcpServer stopped");

            if (this.ServerStopped != null)
            {
                this.ServerStopped();
            }
        }

        public void Disconnect(TcpClient client)
        {
            throw new NotImplementedException();
        }
        
        public void Dispose()
        {
            this.Dispose(true);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual ICoreTcpClient CreateClient()
        {
            return this.factory.Resolve<ICoreTcpClient>();
        }

        protected virtual void ProcessData(CoreTcpData data)
        {
            switch (data.State)
            {
                case TcpDataState.Disconnected:
                case TcpDataState.TimedOut:
                    {
                        if (this.ClientDisconnected != null)
                        {
                            this.ClientDisconnected(data.Client);
                        }

                        break;
                    }
            }
        }
        
        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing && this.isRunning)
            {
                this.Stop();
            }
        }
        
        private void ClientAcceptMain()
        {
            this.listener.Start();

            while (this.canAccept)
            {
                // blocks until a client has connected to the server
                TcpClient client = this.listener.AcceptTcpClient();

                // Create our own client and sync some properties
                var baseTcpClient = this.CreateClient();
                baseTcpClient.SetClient(client);

                Diagnostic.InfoUnmanaged("Client connected: {0}", client.Client.RemoteEndPoint);

                if (this.ClientConnected != null)
                {
                    this.ClientConnected(baseTcpClient);
                }

                CoreTcpUtils.InitiateClientRead(baseTcpClient, this.ProcessData, timeOut: this.ClientTimeout);
            }
        }
    }
}
