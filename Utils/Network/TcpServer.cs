namespace CarbonCore.Utils.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;

    using CarbonCore.Utils.Contracts.Network;

    public class TcpServer : ITcpServer
    {
        private int port;

        private bool isRunning;
        private bool canAccept;

        private TcpListener listener;

        private Thread acceptThread;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public TcpServer()
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action OnServerStarted;

        public event Action OnServerStopped;

        public event TcpServerEventHandler OnClientConnected;

        public event TcpServerEventHandler OnClientDisconnected;

        public int Port
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

        public bool IsRunning
        {
            get
            {
                return this.isRunning;
            }
        }

        public void Start()
        {
            this.listener = new TcpListener(IPAddress.Any, 3000);
            
            this.canAccept = true;
            this.acceptThread = new Thread(this.ClientAcceptMain);
            this.acceptThread.Start();

            this.isRunning = true;
            if (this.OnServerStarted != null)
            {
                this.OnServerStarted();
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

            if (this.OnServerStopped != null)
            {
                this.OnServerStopped();
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
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
                //blocks until a client has connected to the server
                TcpClient client = this.listener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(this.ClientMain);
                clientThread.Start(client);
            }
        }

        private void ClientMain(object arguments)
        {
            var client = (TcpClient)arguments;

            if (this.OnClientConnected != null)
            {
                this.OnClientConnected(client);
            }

            NetworkStream clientStream = client.GetStream();

            // TODO: http://tech.pro/tutorial/704/csharp-tutorial-simple-threaded-tcp-server
            // bytesRead = clientStream.Read(message, 0, 4096);
            // clientStream.Write();


            /*TcpClient tcpClient = (TcpClient)client;
  NetworkStream clientStream = tcpClient.GetStream();

  byte[] message = new byte[4096];
  int bytesRead;

  while (true)
  {
    bytesRead = 0;

    try
    {
      //blocks until a client sends a message
      bytesRead = clientStream.Read(message, 0, 4096);
    }
    catch
    {
      //a socket error has occured
      break;
    }

    if (bytesRead == 0)
    {
      //the client has disconnected from the server
      break;
    }

    //message has successfully been received
    ASCIIEncoding encoder = new ASCIIEncoding();
    System.Diagnostics.Debug.WriteLine(encoder.GetString(message, 0, bytesRead));
  }

  tcpClient.Close();*/
        }
    }
}
