﻿namespace CarbonCore.Utils.Json
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Text;
    
    using CarbonCore.Utils.IO;

    using Newtonsoft.Json;

    using NLog;

    public static class JsonExtensions
    {
        private const int DefaultStreamBufferSize = 4096;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private static readonly IDictionary<Type, Type> GlobalConverterRegistration;

        private static readonly IDictionary<Type, JsonConverterAttribute> LocalConverterRegistration;

        // ------------------------------------------------------------------- 
        // Constructor
        // ------------------------------------------------------------------- 
        static JsonExtensions()
        {
            GlobalConverterRegistration = new Dictionary<Type, Type>();
            LocalConverterRegistration = new Dictionary<Type, JsonConverterAttribute>();
        }

        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public static void RegisterGlobalConverter<T, TN>()
            where TN : JsonConverter
        {
            if (GlobalConverterRegistration.ContainsKey(typeof(T)))
            {
                Logger.Warn("Converter for type {0} already registered to {1}, skipping registration as {2}", typeof(T), GlobalConverterRegistration[typeof(T)], typeof(TN));
                return;
            }

            GlobalConverterRegistration.Add(typeof(T), typeof(TN));
        }

        public static JsonConverter FindConverter<T>()
        {
            return FindConverter(typeof(T));
        }

        public static JsonConverter FindConverter(Type type)
        {
            Type targetType;
            if (GlobalConverterRegistration.TryGetValue(type, out targetType))
            {
                return (JsonConverter)Activator.CreateInstance(targetType, null);
            }

            JsonConverterAttribute attribute;
            if (!LocalConverterRegistration.TryGetValue(type, out attribute))
            {
                object[] converterAttributes = type.GetCustomAttributes(typeof(JsonConverterAttribute), true);
                if (converterAttributes.Length > 0)
                {
                    if (converterAttributes.Length > 1)
                    {
                        throw new InvalidOperationException("More than one possible converter found");
                    }

                    attribute = (JsonConverterAttribute)converterAttributes[0];
                }

                LocalConverterRegistration.Add(type, attribute);
            }
            
            if (attribute == null)
            {
                return null;
            }

            return (JsonConverter)Activator.CreateInstance(attribute.ConverterType, null);
        }

        public static T LoadFromData<T>(string data, params JsonConverter[] converters)
        {
            return JsonConvert.DeserializeObject<T>(data, converters);
        }

        public static T LoadFromByte<T>(byte[] data, bool compressed = true, params JsonConverter[] converters)
        {
            using (var stream = new MemoryStream())
            {
                stream.Write(data, 0, data.Length);
                stream.Seek(0, SeekOrigin.Begin);

                return LoadFromStream<T>(stream, compressed, converters);
            }
        }
        
        public static string SaveToData<T>(T source, Formatting formatting = Formatting.None, DefaultValueHandling defaultHandling = DefaultValueHandling.Ignore, params JsonConverter[] converters)
        {
#if UNITY_5
            // Unity Serializer does not have formatting parameter
            var serializer = new JsonSerializer { DefaultValueHandling = DefaultValueHandling.Ignore };
#else 
            var serializer = new JsonSerializer
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = formatting
            };
#endif

            if (converters != null && converters.Length > 0)
            {
                serializer.Converters.AddRange(converters);
            }

            var builder = new StringBuilder();
            using (var writer = new StringWriter(builder))
            {
                serializer.Serialize(writer, source);
            }

            return builder.ToString();
        }

        public static byte[] SaveToByte<T>(T source, bool compress = true, Formatting formatting = Formatting.None, DefaultValueHandling defaultHandling = DefaultValueHandling.Ignore, params JsonConverter[] converters)
        {
            using (var stream = new MemoryStream())
            {
                SaveToStream(stream, source, compress, formatting, defaultHandling, converters);

                stream.Seek(0, SeekOrigin.Begin);
                var result = new byte[stream.Length];
                stream.Read(result, 0, (int)stream.Length);
                return result;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Checked")]
        public static T LoadFromFile<T>(CarbonFile file, bool compressed = true, params JsonConverter[] converters)
        {
            if (!file.Exists)
            {
                throw new FileNotFoundException("Could not load file", file.ToString());
            }
            
            using (var stream = file.OpenRead())
            {
                return LoadFromStream<T>(stream, compressed, converters);
            }
        }

        public static T LoadFromStream<T>(Stream source, bool compressed = true, params JsonConverter[] converters)
        {
            string rawData;
            if (compressed)
            {
                using (var compressedStream = new GZipStream(source, CompressionMode.Decompress, true))
                {
                    using (var reader = new StreamReader(compressedStream, Encoding.UTF8, false, DefaultStreamBufferSize))
                    {
                        rawData = reader.ReadToEnd();
                    }
                }
            }
            else
            {
                using (var reader = new StreamReader(source, Encoding.UTF8, false, DefaultStreamBufferSize))
                {
                    rawData = reader.ReadToEnd();
                }
            }

            return LoadFromData<T>(rawData, converters);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Checked")]
        public static void SaveToFile<T>(CarbonFile file, T data, bool compress = true, Formatting formatting = Formatting.None, DefaultValueHandling defaultHandling = DefaultValueHandling.Ignore, params JsonConverter[] converters)
        {
            // Check if we need to create directories
            CarbonDirectory directory = file.GetDirectory();
            if (directory != null && !directory.IsNull && !directory.Exists)
            {
                directory.Create();
            }

            // Serialize
            string serialized = SaveToData(data, formatting, defaultHandling, converters);

            // Write to disk either compressed or bare
            using (var stream = file.OpenCreate())
            {
                DoSaveToStream(stream, serialized, compress);
            }
        }

        public static void SaveToStream<T>(Stream target, T data, bool compress = true, Formatting formatting = Formatting.None, DefaultValueHandling defaultHandling = DefaultValueHandling.Ignore, params JsonConverter[] converters)
        {
            // Serialize
            string serialized = SaveToData(data, formatting, defaultHandling, converters);

            DoSaveToStream(target, serialized, compress);
        }

        private static void DoSaveToStream(Stream target, string serializedData, bool compress = true)
        {
            if (compress)
            {
                using (var compressedStream = new GZipStream(target, CompressionMode.Compress, true))
                {
                    using (var writer = new NoDisposeStreamWriter(compressedStream, Encoding.UTF8, DefaultStreamBufferSize))
                    {
                        writer.Write(serializedData);
                    }
                }
            }
            else
            {
                using (var writer = new NoDisposeStreamWriter(target, Encoding.UTF8, DefaultStreamBufferSize))
                {
                    writer.Write(serializedData);
                }
            }
        }
    }
}
