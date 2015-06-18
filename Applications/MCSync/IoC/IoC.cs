namespace CarbonCore.Applications.MCSync.IoC
{
    using CarbonCore.Utils.Compat.IoC;
    using CarbonCore.Utils.IoC;
    using CarbonCore.UtilsCommandLine.IoC;
    
    using MCSync.Contracts;

    [DependsOnModule(typeof(UtilsModule))]
    [DependsOnModule(typeof(UtilsCommandLineModule))]
    public class MCSyncModule : CarbonQuickModule
    {
        public MCSyncModule()
        {
            this.For<IMain>().Use<Main>();
        }
    }
}