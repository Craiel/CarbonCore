namespace CarbonCore.Processing.IoC
{
    using CarbonCore.Utils.IoC;
    using CarbonCore.Processing.Contracts;
    using CarbonCore.Processing.Logic;

    public class CarbonProcessingModule : CarbonModuleAutofac
    {
        public CarbonProcessingModule()
        {
            this.For<IResourceProcessor>().Use<ResourceProcessor>().Singleton();
        }
    }
}