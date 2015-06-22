﻿namespace CarbonCore.Utils.Compat.Contracts.Network
{
    public interface IJsonNetClient : ICoreTcpClient, IJsonNetPeer
    {
        void SendPackage<T>(T package) where T : class, IJsonNetPackage;
    }
}