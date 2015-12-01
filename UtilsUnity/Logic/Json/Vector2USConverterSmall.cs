﻿namespace CarbonCore.Utils.Unity.Logic.Json
{
    using System;
    using System.Diagnostics.CodeAnalysis;

    using CarbonCore.Utils.Unity.Data;

    using Newtonsoft.Json;
    
    public class Vector2USConverterSmall : JsonConverter
    {
        // ------------------------------------------------------------------- 
        // Public 
        // ------------------------------------------------------------------- 
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Vector2US typed = (Vector2US)value;

            writer.WriteStartArray();
            writer.WriteValue(typed.X);
            writer.WriteValue(typed.Y);
            writer.WriteEndArray();
        }

        [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            // Start Array + First Value
            reader.Read();
            ushort x = (ushort)Convert.ChangeType(reader.Value, typeof(ushort));

            reader.Read();
            ushort y = (ushort)Convert.ChangeType(reader.Value, typeof(ushort));

            // End Array
            reader.Read();

            return new Vector2US(x, y);
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Vector2US);
        }
    }
}
