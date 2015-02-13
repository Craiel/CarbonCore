namespace CarbonCore.Utils.IoC
{
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Contracts.Network;
    using CarbonCore.Utils.Formatting;
    using CarbonCore.Utils.Network;

    public class UtilsModule : CarbonModule
    {
        public UtilsModule()
        {
            this.For<IFactory>().Use<Factory>().Singleton();
            this.For<IEventRelay>().Use<EventRelay>().Singleton();

            this.For<IFormatter>().Use<Formatter>();

            this.For<ICoreTcpServer>().Use<CoreTcpServer>();
            this.For<ICoreTcpClient>().Use<CoreTcpClient>();

            this.For<IJsonNetPeer>().Use<JsonNetPeer>();
            this.For<IJsonNetClient>().Use<JsonNetClient>();
            this.For<IJsonNetServer>().Use<JsonNetServer>();
        }
    }
}