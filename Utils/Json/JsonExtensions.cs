namespace CarbonCore.Utils.Json
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    using CarbonCore.Utils.IO;

    using Newtonsoft.Json;

    public static class JsonExtensions
    {
        private const int DefaultStreamBufferSize = 4096;

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public static T LoadFromData<T>(string data)
        {
            return JsonConvert.DeserializeObject<T>(data);
        }

        public static string SaveToData<T>(T source, Formatting formatting = Formatting.None)
        {
            var serializer = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = formatting
            };

            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, source);
            }

            return builder.ToString();
        }

        public static byte[] SaveToByte<T>(T source, bool compress = true, Formatting formatting = Formatting.None)
        {
            using (var stream = new MemoryStream())
            {
                SaveToStream(stream, source, compress, formatting);

                stream.Seek(0, SeekOrigin.Begin);
                var result = new byte[stream.Length];
                stream.Read(result, 0, (int)stream.Length);
                return result;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Checked")]
        public static T LoadFromFile<T>(CarbonFile file, bool compressed = true)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("Could not load file", file.ToString());
            }
            
            using (var stream = file.OpenRead())
            {
                return LoadFromStream<T>(stream, compressed);
            }
        }

        public static T LoadFromStream<T>(Stream source, bool compressed = true)
        {
            string rawData;
            if (compressed)
            {
                using (var compressedStream = new GZipStream(source, CompressionMode.Decompress, true))
                {
                    using (var reader = new StreamReader(compressedStream, Encoding.UTF8, false, DefaultStreamBufferSize, true))
                    {
                        rawData = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (var reader = new StreamReader(source, Encoding.UTF8, false, DefaultStreamBufferSize, true))
                {
                    rawData = reader.ReadToEnd();
                }
            }

            return LoadFromData<T>(rawData);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Checked")]
        public static void SaveToFile<T>(CarbonFile file, T data, bool compress = true, Formatting formatting = Formatting.None)
        {
            // Check if we need to create directories
            CarbonDirectory directory = file.GetDirectory();
            if (directory != null && !directory.IsNull && !directory.Exists)
            {
                directory.Create();
            }

            // Serialize
            string serialized = SaveToData(data, formatting);

            // Write to disk either compressed or bare
            using (var stream = file.OpenCreate())
            {
                DoSaveToStream(stream, serialized, compress);
            }
        }

        public static void SaveToStream<T>(Stream target, T data, bool compress = true, Formatting formatting = Formatting.None)
        {
            // Serialize
            string serialized = SaveToData(data, formatting);

            DoSaveToStream(target, serialized, compress);
        }

        private static void DoSaveToStream(Stream target, string serializedData, bool compress = true)
        {
            if (compress)
            {
                using (var compressedStream = new GZipStream(target, CompressionMode.Compress, true))
                {
                    using (var writer = new StreamWriter(compressedStream, Encoding.UTF8, DefaultStreamBufferSize, true))
                    {
                        writer.Write(serializedData);
                    }
                }
            }
            else
            {
                using (var writer = new StreamWriter(target, Encoding.UTF8, DefaultStreamBufferSize, true))
                {
                    writer.Write(serializedData);
                }
            }
        }
    }
}
