namespace CarbonCore.Utils.Compat.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Utils.Compat.Contracts.Network;
    using CarbonCore.Utils.Compat.Network.Packages;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Bson;

    public static class JsonNetUtils
    {
        public const int PackagePayload = 1;
        public const int PackagePing = 2;

        private static readonly JsonSerializer Serializer = new JsonSerializer();

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static byte[] SerializePackages(IEnumerable<IJsonNetPackage> packages)
        {
            var payload = new JsonNetPackagePayload();
            foreach (IJsonNetPackage package in packages)
            {
                payload.Add(package);
            }

            return SerializePackage(payload);
        }

        public static byte[] SerializePackage<T>(T package)
            where T : class, IJsonNetPackage
        {
            if (package == null)
            {
                throw new ArgumentException("Invalid package");
            }

            using (var stream = new MemoryStream())
            {
                using (var writer = new BsonWriter(stream) { CloseOutput = false })
                {
                    Serializer.Serialize(writer, package);
                }

                var data = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(data, 0, (int)stream.Length);
                return data;
            }
        }

        public static IList<IJsonNetPackage> DeserializePackages(IDictionary<int, Type> packageIdDictionary, byte[] data)
        {
            var payload = DeSerializePackage<JsonNetPackagePayload>(data);
            IList<IJsonNetPackage> result = new List<IJsonNetPackage>();
            for (int i = 0; i < payload.PayloadIds.Count; i++)
            {
                int id = payload.PayloadIds[i];
                if (!packageIdDictionary.ContainsKey(id))
                {
                    throw new InvalidDataException("Unknown package in Payload: {0}" + id);
                }

                Type type = packageIdDictionary[id];
                IJsonNetPackage package = DeSerializePackage(type, payload.Data[i]);
                result.Add(package);
            }

            return result;
        }

        public static T DeSerializePackage<T>(byte[] data)
            where T : class, IJsonNetPackage
        {
            return DeSerializePackage(typeof(T), data) as T;
        }

        public static IJsonNetPackage DeSerializePackage(Type type, byte[] data)
        {
            if (data == null || data.Length <= 0)
            {
                throw new ArgumentException("Invalid data for De-serialize");
            }

            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);
                using (var reader = new BsonReader(stream))
                {
                    return Serializer.Deserialize(reader, type) as IJsonNetPackage;
                }
            }
        }
    }
}
