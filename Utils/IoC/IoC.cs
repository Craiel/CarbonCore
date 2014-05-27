namespace CarbonCore.Utils.IoC
{
    using CarbonCore.Utils.Contracts;
    using CarbonCore.Utils.Contracts.IoC;
    using CarbonCore.Utils.Formatting;

    public class UtilsModule : CarbonModule
    {
        public UtilsModule()
        {
            this.For<IFactory>().Use<Factory>().Singleton();
            this.For<IEventRelay>().Use<EventRelay>().Singleton();

            this.For<IFormatter>().Use<Formatter>();
        }
    }
}