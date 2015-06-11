namespace CarbonCore.Utils.Json
{
    using System;

    using CarbonCore.Utils.Compat.IO;

    using Newtonsoft.Json;

    public class JsonCarbonDirectoryConverter : JsonConverter
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                return;
            }

            var typed = value as CarbonDirectory;
            if (typed == null)
            {
                throw new ArgumentException();
            }

            serializer.Serialize(writer, typed.GetPath());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null || reader.Value == null || reader.Value.GetType() != typeof(string))
            {
                return null;
            }

            var typed = (string)reader.Value;
            if (string.IsNullOrEmpty(typed))
            {
                return null;
            }

            return new CarbonDirectory(typed);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CarbonDirectory);
        }
    }
}
