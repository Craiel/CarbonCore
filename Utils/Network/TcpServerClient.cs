namespace CarbonCore.Utils.Network
{
    using System;

    using CarbonCore.Utils.Contracts.Network;

    public class TcpServerClient : ITcpClient
    {
        private bool isConnected;

        public event Action OnConnected;

        public event Action OnDisconnected;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }
        }

        public void Connect(string address)
        {
            Diagnostics.Internal.NotImplemented();
        }

        public void Disconnect()
        {
            Diagnostics.Internal.NotImplemented();
        }
    }
}
