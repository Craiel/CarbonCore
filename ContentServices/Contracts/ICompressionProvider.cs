namespace CarbonCore.ContentServices.Contracts
{
    using System.IO;

    using CarbonCore.ContentServices.Logic.Enums;

    public interface ICompressionProvider
    {
        void Compress(Stream source, Stream target, CompressionLevel level);

        void Decompress(Stream source, Stream target);
    }
}
