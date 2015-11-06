namespace CarbonCore.Utils.Compat.Network
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Contracts.Network;

    public class JsonNetServer : CoreTcpServer, IJsonNetServer
    {
        private readonly IFactory factory;

        private readonly IJsonNetPeer peer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JsonNetServer(IFactory factory)
            : base(factory)
        {
            this.factory = factory;

            this.peer = factory.Resolve<IJsonNetPeer>();
            this.peer.OnPackageReceived += this.OnPackageReceived;
            this.peer.OnPackageSent += this.OnPackageSent;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event JsonPackageHandler OnPackageReceived;
        public event JsonPackageHandler OnPackageSent;

        public void Register<T>() where T : IJsonNetPackage
        {
            this.peer.Register<T>();
        }

        public void Unregister<T>() where T : IJsonNetPackage
        {
            this.peer.Unregister<T>();
        }

        public byte[] Serialize(IList<IJsonNetPackage> packages)
        {
            return this.peer.Serialize(packages);
        }

        public IList<IJsonNetPackage> Deserialize(byte[] data)
        {
            return this.peer.Deserialize(data);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override ICoreTcpClient CreateClient()
        {
            return this.factory.Resolve<IJsonNetClient>();
        }

        protected override void ProcessData(CoreTcpData data)
        {
            IList<IJsonNetPackage> packages = this.peer.Deserialize(data.Data);
            foreach (IJsonNetPackage package in packages)
            {
                if (this.OnPackageReceived != null)
                {
                    this.OnPackageReceived((IJsonNetClient)data.Client, package);
                }
            }
        }
    }
}
