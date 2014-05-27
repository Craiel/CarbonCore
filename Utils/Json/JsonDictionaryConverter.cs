namespace CarbonCore.Utils.Json
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;

    using Newtonsoft.Json;

    public class DictionaryConverter<T, TN> : JsonConverter
    {
        private const string PropertyNameKey = "0";
        private const string PropertyNameValue = "1";

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
                this.SerializeValue(writer, entry.Key, serializer);
                writer.WritePropertyName(PropertyNameValue);
                this.SerializeValue(writer, entry.Value, serializer);
                writer.WriteEndObject();
            }

            writer.WriteEndArray();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType) || objectType.ImplementsGenericInterface(typeof(IDictionary<,>));
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void SerializeValue(JsonWriter writer, object value, JsonSerializer serializer)
        {
            JsonConverter converter = this.FindConverter(value);
            converter.WriteJson(writer, value, serializer);
        }

        private JsonConverter FindConverter(object data)
        {
            Type keyType = data.GetType();

            object[] converterAttributes = keyType.GetCustomAttributes(typeof(JsonConverterAttribute), true);
            if (converterAttributes.Length <= 0)
            {
                if (keyType.IsClass)
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
