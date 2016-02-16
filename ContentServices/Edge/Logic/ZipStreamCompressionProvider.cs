namespace CarbonCore.ContentServices.Edge.Logic
{
    using System;
    using System.IO;
    using System.IO.Compression;

    using CarbonCore.ContentServices.Contracts;

    public class ZipStreamCompressionProvider : ICompressionProvider
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public void Compress(Stream source, Stream target, ContentServices.Logic.Enums.CompressionLevel level)
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
        private System.IO.Compression.CompressionLevel GetZipStreamLevel(ContentServices.Logic.Enums.CompressionLevel level)
        {
            switch (level)
            {
                case ContentServices.Logic.Enums.CompressionLevel.Optimal:
                    {
                        return System.IO.Compression.CompressionLevel.Optimal;
                    }

                case ContentServices.Logic.Enums.CompressionLevel.Fastest:
                    {
                        return System.IO.Compression.CompressionLevel.Fastest;
                    }

                case ContentServices.Logic.Enums.CompressionLevel.NoCompression:
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
