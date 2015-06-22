namespace CarbonCore.Utils.Compat.IoC
{
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Contracts.Network;
    using CarbonCore.Utils.Compat.Formatting;
    using CarbonCore.Utils.Compat.Network;

    public class UtilsCompatModule : CarbonQuickModule
    {
        public UtilsCompatModule()
        {
            this.For<IFactory>().Use<Factory>().Singleton();
            this.For<IEventRelay>().Use<EventRelay>().Singleton();

            this.For<IFormatter>().Use<Formatter>();

            this.For<ICoreTcpClient>().Use<CoreTcpClient>();
            this.For<ICoreTcpServer>().Use<CoreTcpServer>();

            this.For<IJsonNetPeer>().Use<JsonNetPeer>();
            this.For<IJsonNetClient>().Use<JsonNetClient>();
            this.For<IJsonNetServer>().Use<JsonNetServer>();
        }
    }
}