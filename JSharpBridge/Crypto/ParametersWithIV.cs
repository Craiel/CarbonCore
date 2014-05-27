namespace CarbonCore.JSharpBridge.Crypto
{
    public class ParametersWithIV : KeyGenerationParameters
    {
        public ParametersWithIV(SecureRandom random, int unknown)
            : base(random, unknown)
        {
        }

        public ParametersWithIV(KeyParameter random, byte[] getEncoded, int i, int i1)
            : base(null, 0)
        {
            throw new System.NotImplementedException();
        }
    }
}
