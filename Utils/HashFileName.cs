namespace CarbonCore.Utils
{
    using System;
    using System.Globalization;
    using System.Security.Cryptography;
    using System.Text;

    public enum HashFileNameMethod
    {
        SHA1,
        FNV
    }

    public struct HashFileName
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public HashFileName(string hash, HashFileNameMethod method)
            : this()
        {
            this.Value = hash;
            this.Method = method;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Value { get; private set; }

        public HashFileNameMethod Method { get; set; }
        
        public static HashFileName GetHashFileName(string data, HashFileNameMethod method = HashFileNameMethod.FNV)
        {
            return GetHashFileName(Encoding.UTF8.GetBytes(data), method);
        }

        public static HashFileName GetHashFileName(int value, HashFileNameMethod method = HashFileNameMethod.FNV)
        {
            return GetHashFileName(BitConverter.GetBytes(value), method);
        }

        public override bool Equals(object obj)
        {
            var typed = (HashFileName)obj;
            if (string.IsNullOrEmpty(typed.Value))
            {
                return string.IsNullOrEmpty(this.Value);
            }

            return typed.Value.Equals(this.Value);
        }

        public override int GetHashCode()
        {
            return Tuple.Create(this.Value).GetHashCode();
        }

        public override string ToString()
        {
            return this.Value;
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private static HashFileName GetHashFileName(byte[] data, HashFileNameMethod method)
        {
            switch (method)
            {
                case HashFileNameMethod.SHA1:
                    {
                        var sha = SHA1.Create();
                        byte[] hash = sha.ComputeHash(data);
                        string hashString = Convert.ToBase64String(hash).Replace('/', '-');
                        return new HashFileName(hashString, HashFileNameMethod.SHA1);
                    }

                case HashFileNameMethod.FNV:
                    {
                        var fnv = FNV.Create();
                        int hash = fnv.Compute(data);
                        string hashString = hash.ToString(CultureInfo.InvariantCulture).Replace('-', 'N');
                        return new HashFileName(hashString, HashFileNameMethod.FNV);
                    }

                default:
                    {
                        return Utils.Diagnostics.Internal.NotImplemented<HashFileName>();
                    }
            }
        }
    }
}
