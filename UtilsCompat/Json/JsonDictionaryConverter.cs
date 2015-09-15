namespace CarbonCore.Utils.Compat.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using CarbonCore.Utils.Compat.Diagnostics;

    using Newtonsoft.Json;

    public class JsonDictionaryConverter<T, TN> : JsonConverter
    {
        private const string PropertyNameKey = "0";
        private const string PropertyNameValue = "1";

        private readonly JsonConverter keyConverter;
        private readonly JsonConverter valueConverter;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public JsonDictionaryConverter(JsonConverter keyConverter = null, JsonConverter valueConverter = null)
        {
            this.keyConverter = keyConverter;
            this.valueConverter = valueConverter;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            // Check if there is something to write
            if (value == null)
            {
                return;
            }

            var dictionary = value as IDictionary<T, TN>;
            if (dictionary == null)
            {
                throw new ArgumentException();
            }

            // Skip empty dictionaries
            if (dictionary.Count <= 0)
            {
                return;
            }

            writer.WriteStartArray();
            foreach (KeyValuePair<T, TN> entry in dictionary)
            {
                writer.WriteStartObject();
                writer.WritePropertyName(PropertyNameKey);
                this.SerializeValue(writer, entry.Key, serializer, this.keyConverter);
                writer.WritePropertyName(PropertyNameValue);
                this.SerializeValue(writer, entry.Value, serializer, this.valueConverter);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            System.Diagnostics.Trace.Assert(reader.TokenType == JsonToken.StartArray);

            var resultDictionary = Activator.CreateInstance(objectType, null);

            Type[] genericArguments = objectType.GetGenericArguments();
            System.Diagnostics.Trace.Assert(genericArguments != null && genericArguments.Length == 2);

            Type keyType = objectType.GetGenericArguments()[0];
            Type valueType = objectType.GetGenericArguments()[1];

            try
            {
                // Start array
                reader.Read();
                while (reader.TokenType != JsonToken.EndArray)
                {
                    // Start object
                    reader.Read();

                    // Key
                    // Note: This is a little weird, for the first one we have to read directly without skipping to the value
                    //       ReadString() of the "0" key will skip the value as well...
                    System.Diagnostics.Trace.Assert(reader.Value.Equals("0"));
                    var keyValue = this.DeserializeValue(reader, keyType, existingValue, serializer, this.keyConverter);
                    reader.Read();

                    // Value
                    // Note: For this we have to first read the key token, then read the post value after
                    System.Diagnostics.Trace.Assert(reader.Value.Equals("1"));
                    reader.Read();
                    var valueValue = this.DeserializeValue(reader, valueType, existingValue, serializer, this.valueConverter);
                    reader.Read();

                    // End object
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
        private object DeserializeValue(JsonReader reader, Type type, object existingValue, JsonSerializer serializer, JsonConverter customConverter)
        {
            JsonConverter converter = customConverter ?? this.FindConverter(type);
            return converter.ReadJson(reader, type, existingValue, serializer);
        }

        private void SerializeValue(JsonWriter writer, object value, JsonSerializer serializer, JsonConverter customConverter)
        {
            JsonConverter converter = customConverter ?? this.FindConverter(value.GetType());
            converter.WriteJson(writer, value, serializer);
        }

        private JsonConverter FindConverter(Type type)
        {
            object[] converterAttributes = type.GetCustomAttributes(typeof(JsonConverterAttribute), true);
            if (converterAttributes.Length <= 0)
            {
                if (type.IsClass)
                {
                    return new JsonDefaultClassStringConverter();
                }
                
                return new JsonDefaultConverter();
            }

            if (converterAttributes.Length > 1)
            {
                throw new InvalidDataException("More than one possible converter found");
            }

            return (JsonConverter)Activator.CreateInstance(((JsonConverterAttribute)converterAttributes[0]).ConverterType, null);
        }
    }
}
