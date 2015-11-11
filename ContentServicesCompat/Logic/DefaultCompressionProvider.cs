namespace CarbonCore.ContentServices.Compat.Logic
{
    using System.IO;

    using CarbonCore.ContentServices.Compat.Contracts;
    using CarbonCore.ContentServices.Compat.Logic.Enums;

    public class DefaultCompressionProvider : ICompressionProvider
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Compress(Stream source, Stream target, CompressionLevel level)
        {
            // Todo: add a simple but compatible compression
            source.Seek(0, SeekOrigin.Begin);
            target.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[source.Length];
            source.Read(buffer, 0, buffer.Length);
            target.Write(buffer, 0, buffer.Length);
        }

        public void Decompress(Stream source, Stream target)
        {
            source.Seek(0, SeekOrigin.Begin);
            target.Seek(0, SeekOrigin.Begin);

            byte[] buffer = new byte[source.Length];
            source.Read(buffer, 0, buffer.Length);
            target.Write(buffer, 0, buffer.Length);
        }
    }
}
