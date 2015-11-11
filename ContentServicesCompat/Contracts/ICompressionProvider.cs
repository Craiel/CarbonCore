namespace CarbonCore.ContentServices.Compat.Contracts
{
    using System.IO;

    using CarbonCore.ContentServices.Compat.Logic.Enums;

    public interface ICompressionProvider
    {
        void Compress(Stream source, Stream target, CompressionLevel level);

        void Decompress(Stream source, Stream target);
    }
}
