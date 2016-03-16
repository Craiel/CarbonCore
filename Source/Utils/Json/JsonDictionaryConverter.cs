namespace CarbonCore.Utils.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;

    using Newtonsoft.Json;

    public class JsonDictionaryConverter<T, TN> : JsonConverter
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Check if there is something to write
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            var dictionary = value as IDictionary<T, TN>;
            if (dictionary == null)
            {
                throw new ArgumentException(string.Format("Expected Dictionary of {0} but got {1}", typeof(IDictionary<T, TN>), value.GetType()));
            }

            writer.WriteStartArray();
            foreach (KeyValuePair<T, TN> entry in dictionary)
            {
                writer.WriteStartArray();
                this.SerializeValue(writer, entry.Key, serializer);
                this.SerializeValue(writer, entry.Value, serializer);
                writer.WriteEndArray();
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            Diagnostic.Assert(reader.TokenType == JsonToken.StartArray);

            var resultDictionary = Activator.CreateInstance(objectType, null);

            Type[] genericArguments = objectType.GetGenericArguments();
            Diagnostic.Assert(genericArguments != null && genericArguments.Length == 2);

            Type keyType = objectType.GetGenericArguments()[0];
            Type valueType = objectType.GetGenericArguments()[1];

            try
            {
                // Start array
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    // Start array
                    reader.Read();

                    // Key
                    var keyValue = this.DeserializeValue(reader, keyType, existingValue, serializer);

                    // End Object
                    reader.Read();

                    // Value
                    var valueValue = this.DeserializeValue(reader, valueType, existingValue, serializer);

                    // End object
                    reader.Read();

                    // End Array
                    reader.Read();

                    ((IDictionary)resultDictionary).Add(keyValue, valueValue);
                }

                // Note Here we are at EndArray token, do not skip or read that one here!
            }
            catch (Exception e)
            {
                Diagnostic.Exception(e);
                Diagnostic.Error("Error reading Json Dictionary");
            }

            return resultDictionary;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType) || objectType.ImplementsGenericInterface(typeof(IDictionary<,>));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private object DeserializeValue(JsonReader reader, Type type, object existingValue, JsonSerializer serializer)
        {
            JsonConverter converter = JsonExtensions.FindConverter(type);
            if (converter != null)
            {
                return converter.ReadJson(reader, type, existingValue, serializer);
            }

            return serializer.Deserialize(reader, type);
        }

        private void SerializeValue(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonConverter converter = JsonExtensions.FindConverter(value.GetType());
            if (converter != null)
            {
                converter.WriteJson(writer, value, serializer);
                return;
            }

            serializer.Serialize(writer, value);
        }
    }
}
