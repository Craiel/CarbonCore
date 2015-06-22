namespace CarbonCore.Utils.Compat.Json
{
    using System;

    using Newtonsoft.Json;

    // This uses the default serialization and is used to forward simple object serialization
    public class JsonDefaultConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }
    }
}
