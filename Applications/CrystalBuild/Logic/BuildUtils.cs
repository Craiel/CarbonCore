namespace CarbonCore.Applications.CrystalBuild.Logic
{
    using System.Linq;

    using CrystalBuild.Contracts;

    public class BuildUtils : IBuildUtils
    {
        public string Compress(string data)
        {
            var encoder = new SharpLZW.LZWEncoder();
            return System.Convert.ToBase64String(encoder.EncodeToByteList(data).ToArray());
        }
    }
}
