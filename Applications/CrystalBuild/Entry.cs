namespace CarbonCore.Applications.CrystalBuild
{
    using CarbonCore.ContentServices.Logic.DataEntryLogic;
    using CarbonCore.Tests.ContentServices;
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.IoC;

    using CrystalBuild.Contracts;
    using CrystalBuild.IoC;

    public static class Entry
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public static void Main(string[] args)
        {
            int cycles = 50000;
            var clone = (DataTestEntry)DataTestData.FullTestEntry.Clone();

            int totalData = 0;
            for (var i = 0; i < cycles; i++)
            {
                using (new ProfileRegion("DataEntry.CompactSerialization"))
                {
                    byte[] data = DataEntrySerialization.CompactSave(clone);
                    totalData += data.Length;
                    DataEntrySerialization.CompactLoad<DataTestEntry>(data);
                }
            }

            //var container = CarbonContainerAutofacBuilder.Build<CrystalBuildModule>();
            //container.Resolve<IMain>().Build();

            //Profiler.TraceProfilerStatistics();
        }
    }
}