namespace CarbonCore.Utils.Compat.Network
{
    using CarbonCore.Utils.Compat.Contracts.Network;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public abstract class JsonNetPackage : IJsonNetPackage
    {
        [JsonIgnore]
        public abstract int Id { get; }
    }
}
