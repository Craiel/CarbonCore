namespace CarbonCore.Utils.IoC
{
    using CarbonCore.Utils.Compat.Contracts.Network;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.Contracts.Network;
    using CarbonCore.Utils.Network;

    [DependsOnModule(typeof(UtilsCompatModule))]
    public class UtilsModule : CarbonQuickModule
    {
        public UtilsModule()
        {
            this.For<ICoreTcpServer>().Use<CoreTcpServer>();

            this.For<IJsonNetPeer>().Use<JsonNetPeer>();
            this.For<IJsonNetClient>().Use<JsonNetClient>();
            this.For<IJsonNetServer>().Use<JsonNetServer>();
        }
    }
}