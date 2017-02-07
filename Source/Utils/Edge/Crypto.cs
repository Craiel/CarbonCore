namespace CarbonCore.Utils.Edge
{
    using System;
    using System.Security.Cryptography;

    public static class Crypto
    {
        private static readonly char[] Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        public static string GetUniqueKey(int size)
        {
            byte[] data;
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                data = new byte[4 * size];
                crypto.GetBytes(data);
            }

            char[] result = new char[size];
            for (var i = 0; i < size; i++)
            {
                result[i] = Chars[(uint)BitConverter.ToInt32(data, 4 * i) % Chars.Length];
            }

            return new string(result);
        }
    }
}
