namespace CarbonCore.Utils.Unity.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    using UnityEngine;

    public class ColorConverter : JsonConverter
    {
        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Color typed = (Color)value;

            writer.WriteStartArray();
            writer.WriteValue(typed.r);
            writer.WriteValue(typed.g);
            writer.WriteValue(typed.b);
            writer.WriteValue(typed.a);
            writer.WriteEndArray();
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Start Array + First Value
            reader.Read();
            float r = (float)Convert.ChangeType(reader.Value, typeof(float));

            reader.Read();
            float g = (float)Convert.ChangeType(reader.Value, typeof(float));

            reader.Read();
            float b = (float)Convert.ChangeType(reader.Value, typeof(float));

            reader.Read();
            float a = (float)Convert.ChangeType(reader.Value, typeof(float));

            // End Array
            reader.Read();

            return new Color(r, g, b, a);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Color);
        }
    }
}
