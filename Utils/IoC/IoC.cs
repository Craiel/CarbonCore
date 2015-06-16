namespace CarbonCore.Utils.IoC
{
    using CarbonCore.Utils.Compat;
    using CarbonCore.Utils.Compat.Contracts;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Contracts.Network;
    using CarbonCore.Utils.Compat.Formatting;
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.Compat.Network;
    using CarbonCore.Utils.Contracts.Network;
    using CarbonCore.Utils.Network;

    public class UtilsModule : CarbonQuickModule
    {
        public UtilsModule()
        {
            this.For<IFactory>().Use<FactoryAutofac>().Singleton();
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