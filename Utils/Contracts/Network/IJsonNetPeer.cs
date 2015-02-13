namespace CarbonCore.Utils.Contracts.Network
{
    using System.Collections.Generic;

    public delegate void JsonPackageHandler(IJsonNetClient client, IJsonNetPackage package);

    public interface IJsonNetPeer
    {
        event JsonPackageHandler OnPackageReceived;
        event JsonPackageHandler OnPackageSent;

        void Register<T>() where T : IJsonNetPackage;
        void Unregister<T>() where T : IJsonNetPackage;

        byte[] Serialize(IList<IJsonNetPackage> packages);
        IList<IJsonNetPackage> Deserialize(byte[] data);
    }
}
