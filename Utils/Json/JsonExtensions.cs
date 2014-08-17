namespace CarbonCore.Utils.Json
{
    using System.IO;
    using System.IO.Compression;
    using System.Text;

    using CarbonCore.Utils.IO;

    using Newtonsoft.Json;

    public static class JsonExtensions
    {
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

        public static T LoadFromFile<T>(CarbonFile file, bool compressed = true)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("Could not load file", file.ToString());
            }
            
            string rawData;
            using (var stream = file.OpenRead())
            {
                if (compressed)
                {
                    using (var compressedStream = new GZipStream(stream, CompressionMode.Decompress))
                    {
                        using (var reader = new StreamReader(compressedStream))
                        {
                            rawData = reader.ReadToEnd();
                        }
                    }
                }
                else
                {
                    using (var reader = new StreamReader(stream))
                    {
                        rawData = reader.ReadToEnd();
                    }
                }
            }

            return LoadFromData<T>(rawData);
        }

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
                if (compress)
                {
                    using (var compressedStream = new GZipStream(stream, CompressionMode.Compress))
                    {
                        using (var writer = new StreamWriter(compressedStream))
                        {
                            writer.Write(serialized);
                        }
                    }
                }
                else
                {
                    using (var writer = new StreamWriter(stream))
                    {
                        writer.Write(serialized);
                    }
                }
            }
        }
    }
}
