namespace CarbonCore.Utils.Network
{
    using CarbonCore.Utils.Contracts.Network;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public abstract class JsonNetPackage : IJsonNetPackage
    {
        [JsonIgnore]
        public abstract int Id { get; }
    }
}
