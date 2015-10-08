namespace D3Theory.IoC
{
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;

    using D3Theory.Contracts;

    [DependsOnModule(typeof(UtilsModule))]
    public class D3TheoryModule : CarbonQuickModule
    {
        public D3TheoryModule()
        {
            this.For<IMainData>().Use<MainData>().Singleton();
        }
    }
}