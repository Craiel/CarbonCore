namespace CarbonCore.Utils.Compat
{
    using System;
    using System.Security.Cryptography;

    public class FNV : HashAlgorithm
    {
        private const int P = 16777619;
        private const uint P2 = 2166136261;

        private byte[] data;

        public static new FNV Create()
        {
            return new FNV();
        }

        public static int Combine(byte[][] data)
        {
            unchecked
            {
                var hash = (int)P2;
                for (int i = 0; i < data.Length; i++)
                {
                    hash = Compute(hash, data[i]);
                }

                return hash;
            }
        }

        public int Compute(byte[] customData)
        {
            unchecked
            {
                return Compute((int)P2, customData);
            }
        }

        public override void Initialize()
        {
        }

        protected override void HashCore(byte[] array, int start, int size)
        {
            this.data = new byte[size];
            Buffer.BlockCopy(array, start, this.data, 0, size);
        }

        protected override byte[] HashFinal()
        {
            unchecked
            {
                return BitConverter.GetBytes(Compute((int)P2, this.data));
            }
        }

        private static int Compute(int hash, byte[] data)
        {
            unchecked
            {
                for (int i = 0; i < data.Length; i++)
                {
                    hash = (hash ^ data[i]) * P;
                }

                hash += hash << 13;
                hash ^= hash >> 7;
                hash += hash << 3;
                hash ^= hash >> 17;
                hash += hash << 5;
                return hash;
            }
        }
    }
}
