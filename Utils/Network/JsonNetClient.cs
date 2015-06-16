namespace CarbonCore.Utils.Network
{
    using System.Collections.Generic;

    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Network;
    using CarbonCore.Utils.Contracts.Network;

    public class JsonNetClient : CoreTcpClient, IJsonNetClient
    {
        private readonly IJsonNetPeer peer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public JsonNetClient(IFactory factory)
        {
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

        public void SendPackage<T>(T package) where T : class, IJsonNetPackage
        {
            byte[] data = this.peer.Serialize(new List<IJsonNetPackage> { package });
            this.Send(data);

            if (this.OnPackageSent != null)
            {
                this.OnPackageSent(this, package);
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected override void ProcessData(CoreTcpData data)
        {
            if (data.BytesRead <= 0)
            {
                return;
            }

            if (data.BytesRead >= 8100)
            {
                Diagnostics.Internal.NotImplemented("Large packages are not yet supported, implement!");
            }

            IList<IJsonNetPackage> packages = this.peer.Deserialize(data.Data);
            foreach (IJsonNetPackage package in packages)
            {
                if (this.OnPackageReceived != null)
                {
                    this.OnPackageReceived(this, package);
                }
            }
        }
    }
}
