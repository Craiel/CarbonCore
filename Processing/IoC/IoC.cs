namespace CarbonCore.Processing.IoC
{
    using CarbonCore.Processing.Contracts;
    using CarbonCore.Processing.Logic;
    using CarbonCore.Utils.Compat.IoC;

    public class CarbonProcessingModule : CarbonQuickModule
    {
        public CarbonProcessingModule()
        {
            this.For<IResourceProcessor>().Use<ResourceProcessor>().Singleton();
        }
    }
}