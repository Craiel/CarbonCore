namespace CarbonCore.Unity.Utils.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    using UnityEngine;

    public class QuaternionConverterSmall : JsonConverter
    {
        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Quaternion typed = (Quaternion)value;

            writer.WriteStartArray();
            writer.WriteValue(typed.x);
            writer.WriteValue(typed.y);
            writer.WriteValue(typed.z);
            writer.WriteValue(typed.w);
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

            reader.Read();
            float z = (float)Convert.ChangeType(reader.Value, typeof(float));

            reader.Read();
            float w = (float)Convert.ChangeType(reader.Value, typeof(float));

            // End Array
            reader.Read();

            return new Quaternion(x, y, z, w);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Quaternion);
        }
    }
}
