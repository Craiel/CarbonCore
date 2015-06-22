namespace CarbonCore.Utils.Compat.Network.Packages
{
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
