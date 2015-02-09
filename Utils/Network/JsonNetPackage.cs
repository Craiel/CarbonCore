namespace CarbonCore.Utils.Network
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public abstract class JsonNetPackage
    {
        [JsonIgnore]
        public abstract int Id { get; }
    }
}
