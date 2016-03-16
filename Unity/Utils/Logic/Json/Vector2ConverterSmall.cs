namespace CarbonCore.Unity.Utils.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    using UnityEngine;

    public class Vector2ConverterSmall : JsonConverter
    {
        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector2 typed = (Vector2)value;

            writer.WriteStartArray();
            writer.WriteValue(typed.x);
            writer.WriteValue(typed.y);
            writer.WriteEndArray();
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Start Array + First Value
            reader.Read();
            float x = (float)Convert.ChangeType(reader.Value, typeof(float));

            reader.Read();
            float y = (float)Convert.ChangeType(reader.Value, typeof(float));
            
            // End Array
            reader.Read();

            return new Vector2(x, y);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2);
        }
    }
}
