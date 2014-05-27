namespace CarbonCore.JSharpBridge.Crypto
{
    public class KeyParameter : KeyGenerationParameters
    {
        public KeyParameter(byte[] random)
            : base(null, 0)
        {
        }

        public KeyParameter(SecureRandom random, int unknown)
            : base(random, unknown)
        {
        }
    }
}
