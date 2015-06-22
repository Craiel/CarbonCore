namespace CarbonCore.Utils.Compat.Json
{
    using System;

    using Newtonsoft.Json;

    // http://blog.greatrexpectations.com/2012/08/30/deserializing-interface-properties-using-json-net/
    public class JsonConcreteTypeConverter<T> : JsonConverter
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<T>(reader);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value);
        }
    }
}
