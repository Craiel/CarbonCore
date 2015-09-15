﻿namespace CarbonCore.Utils.Unity.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using Newtonsoft.Json;

    using UnityEngine;

    public class Vector3ConverterSmall : JsonConverter
    {
        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector3 typed = (Vector3)value;

            writer.WriteStartArray();
            writer.WriteValue(typed.x);
            writer.WriteValue(typed.y);
            writer.WriteValue(typed.z);
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

            // End Array
            reader.Read();

            return new Vector3(x, y, z);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector3);
        }
    }
}
