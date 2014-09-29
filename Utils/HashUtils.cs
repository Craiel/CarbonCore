namespace CarbonCore.Utils
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public static class HashUtils
    {
        private static readonly SHA1 HashProvider = SHA1.Create();
        private static readonly MD5 Md5Provider = MD5.Create();

        public static string BuildResourceHash(string path)
        {
            lock (HashProvider)
            {
                byte[] hashData = HashProvider.ComputeHash(Encoding.UTF8.GetBytes(path));
                return Convert.ToBase64String(hashData);
            }
        }

        public static int CombineObjectHashes(object[] data)
        {
            var hashCodes = new byte[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                hashCodes[i] = BitConverter.GetBytes(data[i] == null ? 0 : data[i].GetHashCode());
            }

            return FNV.Combine(hashCodes);
        }

        public static int CombineObjectHashes(IEnumerable data)
        {
            IList<byte[]> hashCodes = new List<byte[]>();
            foreach (object o in data)
            {
                hashCodes.Add(BitConverter.GetBytes(o == null ? 0 : o.GetHashCode()));
            }

            return FNV.Combine(hashCodes.ToArray());
        }

        public static int CombineHashes<T>(T[] data)
            where T : struct 
        {
            if (data == null)
            {
                return 0;
            }

            var hashCodes = new byte[data.Length][];
            for (int i = 0; i < data.Length; i++)
            {
                hashCodes[i] = BitConverter.GetBytes(data[i].GetHashCode());
            }

            return FNV.Combine(hashCodes);
        }

        public static byte[] GetMd5(Stream source)
        {
            source.Position = 0;
            lock (Md5Provider)
            {
                return Md5Provider.ComputeHash(source);
            }
        }

        public static string Md5ToString(byte[] data)
        {
            return Convert.ToBase64String(data);
        }
    }
}
