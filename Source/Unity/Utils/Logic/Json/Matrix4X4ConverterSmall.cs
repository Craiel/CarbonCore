namespace CarbonCore.Unity.Utils.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    using UnityEngine;

    public class Matrix4x4ConverterSmall : JsonConverter
    {
        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Matrix4x4 typed = (Matrix4x4)value;

            writer.WriteStartArray();
            for (var i = 0; i < 16; i++)
            {
                writer.WriteValue(typed[i]);
            }
            writer.WriteEndArray();
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var result = new Matrix4x4();

            // Start Array + First Value
            for (var i = 0; i < 16; i++)
            {
                reader.Read();
                result[i] = (float)Convert.ChangeType(reader.Value, typeof(float));
            }

            // End Array
            reader.Read();

            return result;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Matrix4x4);
        }
    }
}
