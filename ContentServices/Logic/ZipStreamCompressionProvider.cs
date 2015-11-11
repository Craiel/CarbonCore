namespace CarbonCore.ContentServices.Logic
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using CarbonCore.ContentServices.Compat.Contracts;

    using CompressionLevel = CarbonCore.ContentServices.Compat.Logic.Enums.CompressionLevel;

    public class ZipStreamCompressionProvider : ICompressionProvider
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Compress(Stream source, Stream target, CompressionLevel level)
        {
            using (var zipStream = new GZipStream(target, this.GetZipStreamLevel(level), true))
            {
                source.CopyTo(zipStream);

                // To ensure the data is written properly
                zipStream.Close();
            }
        }

        public void Decompress(Stream source, Stream target)
        {
            using (var zipStream = new GZipStream(source, CompressionMode.Decompress, true))
            {
                zipStream.CopyTo(target);
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private System.IO.Compression.CompressionLevel GetZipStreamLevel(CompressionLevel level)
        {
            switch (level)
            {
                case CompressionLevel.Optimal:
                    {
                        return System.IO.Compression.CompressionLevel.Optimal;
                    }

                case CompressionLevel.Fastest:
                    {
                        return System.IO.Compression.CompressionLevel.Fastest;
                    }

                case CompressionLevel.NoCompression:
                    {
                        return System.IO.Compression.CompressionLevel.NoCompression;
                    }

                default:
                    {
                        throw new NotImplementedException();
                    }
            }
        }
    }
}
