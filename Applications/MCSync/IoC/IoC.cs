namespace MCSync.IoC
{
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsCommandLine.IoC;
    
    using MCSync.Contracts;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    public class MCSyncModule : CarbonModule
    {
        public MCSyncModule()
        {
            this.For<IMain>().Use<Main>();
        }
    }
}