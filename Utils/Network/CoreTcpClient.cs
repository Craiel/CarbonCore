namespace CarbonCore.Utils.Network
{
    using System;
    using System.Net;
    using System.Net.Sockets;

    using CarbonCore.Utils.Contracts.Network;

    public class CoreTcpClient : ICoreTcpClient
    {
        private bool isConnected;

        private TcpClient client;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event TcpClientEventHandler OnConnected;

        public event TcpClientEventHandler OnDisconnected;

        public long BytesSent { get; protected set; }
        
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        public IPEndPoint EndPoint { get; private set; }

        public void Connect(IPEndPoint endPoint)
        {
            this.EndPoint = endPoint;

            this.client = new TcpClient();
            this.client.Connect(endPoint);

            if (!this.client.Connected)
            {
                throw new ArgumentException("Could not connect to endpoint: " + endPoint);
            }

            this.isConnected = true;

            if (this.OnConnected != null)
            {
                this.OnConnected(this);
            }
        }

        public void Disconnect()
        {
            if (!this.isConnected)
            {
                throw new InvalidOperationException("Client is not connected");
            }

            this.client.Close();
            this.client = null;

            this.isConnected = false;
        }

        public NetworkStream GetStream()
        {
            System.Diagnostics.Trace.Assert(this.IsConnected);

            if (this.client == null || !this.client.Connected)
            {
                return null;
            }

            return this.client.GetStream();
        }

        public void Close()
        {
            System.Diagnostics.Trace.Assert(this.IsConnected);

            this.client.Close();
        }

        public void SetClient(TcpClient newClient)
        {
            if (this.isConnected)
            {
                throw new InvalidOperationException("Can not change underlying client while connected");
            }

            this.client = newClient;
            this.EndPoint = (IPEndPoint)newClient.Client.RemoteEndPoint;
            this.isConnected = newClient.Connected;
        }

        public void Send(byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Can not send null or empty data");
            }

            System.Diagnostics.Trace.Assert(this.IsConnected);

            CoreTcpUtils.InitiateClientWrite(this, data, this.OnSendCompleted);
        }

        public void Receive()
        {
            CoreTcpUtils.InitiateClientRead(this, this.ProcessData);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void ProcessData(CoreTcpData data)
        {
            switch (data.State)
            {
                case TcpDataState.Disconnected:
                case TcpDataState.TimedOut:
                    {
                        if (this.OnDisconnected != null)
                        {
                            this.OnDisconnected(this);
                        }

                        break;
                    }
            }
        }

        protected void OnSendCompleted(CoreTcpData data)
        {
            System.Diagnostics.Debug.WriteLine("TcpClient Send completed for {0} bytes", data.Data.Length.ToString());
        }
    }
}
