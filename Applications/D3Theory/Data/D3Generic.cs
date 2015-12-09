namespace CarbonCore.Modules.D3Theory.Data
{
    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class D3Generic
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public float ActionDelay { get; set; }
    }
}
