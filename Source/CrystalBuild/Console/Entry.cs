namespace CarbonCore.CrystalBuild.Console
{
    using Applications.CrystalBuild.Contracts;

    using IoC;
    
    using Utils.Edge.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            var container = CarbonContainerAutofacBuilder.Build<CrystalBuildCSharpModule>();
            container.Resolve<IMain>().Start();
        }
    }
}