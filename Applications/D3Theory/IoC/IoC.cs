namespace CarbonCore.Modules.D3Theory.IoC
{
    using CarbonCore.Modules.D3Theory.Contracts;
    using CarbonCore.Utils.Edge.IoC;
    using CarbonCore.Utils.IoC;
    
    [DependsOnModule(typeof(UtilsEdgeModule))]
    public class D3TheoryModule : CarbonQuickModule
    {
        public D3TheoryModule()
        {
            this.For<IMainData>().Use<MainData>().Singleton();
        }
    }
}