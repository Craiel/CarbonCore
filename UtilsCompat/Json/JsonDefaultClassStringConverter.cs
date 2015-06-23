namespace CarbonCore.Utils.Compat.Json
{
    using System;
    using System.Text;

    using Newtonsoft.Json;

    public class JsonDefaultClassStringConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string rawValue = Encoding.ASCII.GetString(reader.ReadAsBytes());
            if (objectType == typeof(string))
            {
                return rawValue;
            }

            return Activator.CreateInstance(objectType, rawValue);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsClass;
        }
    }
}
