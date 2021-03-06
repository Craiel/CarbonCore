﻿namespace CarbonCore.Utils.Network
{
    using System;
    using System.Collections.Generic;
    
    using CarbonCore.Utils.Contracts.Network;
    using CarbonCore.Utils.Network.Package;

    using NLog;

    public class JsonNetPeer : IJsonNetPeer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDictionary<int, Type> packageDictionary;
        private readonly IDictionary<Type, int> packageIdDictionary;

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        public JsonNetPeer()
        {
            this.packageDictionary = new Dictionary<int, Type>();
            this.packageIdDictionary = new Dictionary<Type, int>();

            // Register some default and internal package types
            this.Register<JsonNetPackagePayload>();
            this.Register<JsonNetPackagePing>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
#pragma warning disable 0067
        public event JsonPackageHandler OnPackageReceived;
        public event JsonPackageHandler OnPackageSent;
#pragma warning restore 0067

        public void Register<T>() where T : IJsonNetPackage
        {
            Type type = typeof(T);
            if (this.packageIdDictionary.ContainsKey(type))
            {
                throw new ArgumentException("Type is already registered: " + type);
            }

            var instance = Activator.CreateInstance<T>();
            int id = instance.Id;
            if (this.packageDictionary.ContainsKey(id))
            {
                throw new ArgumentException(string.Format("Type with same id was already registered: {0} - {1}", id, this.packageDictionary[id]));
            }

            this.packageDictionary.Add(id, type);
            this.packageIdDictionary.Add(type, id);
        }

        public void Unregister<T>() where T : IJsonNetPackage
        {
            Type type = typeof(T);
            int packageId;
            if (!this.packageIdDictionary.TryGetValue(type, out packageId))
            {
                throw new ArgumentException("Type was not registered: " + type);
            }

            this.packageDictionary.Remove(packageId);
            this.packageIdDictionary.Remove(type);
        }

        public byte[] Serialize(IList<IJsonNetPackage> packages)
        {
            return JsonNetUtils.SerializePackages(packages);
        }

        public IList<IJsonNetPackage> Deserialize(byte[] data)
        {
            return JsonNetUtils.DeserializePackages(this.packageDictionary, data);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void ProcessData(int id, byte[] data)
        {
            if (!this.packageDictionary.ContainsKey(id))
            {
                throw new ArgumentException("Package not registered: " + id);
            }

            IJsonNetPackage package = JsonNetUtils.DeSerializePackage(this.packageDictionary[id], data);
            Logger.Info("Deserialized Package {0}", package.GetType());
        }
    }
}
