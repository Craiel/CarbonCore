namespace CarbonCore.Utils.Network.Package
{
    using CarbonCore.Utils.Network;

    using Newtonsoft.Json;

    [JsonObject(MemberSerialization.OptOut)]
    public class JsonNetPackagePing : JsonNetPackage
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override int Id
        {
            get
            {
                return JsonNetUtils.PackagePing;
            }
        }
    }
}
