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

            int totalData = 0;
            for (var i = 0; i < cycles; i++)
            {
                using (new ProfileRegion("DataEntry.CompactSerialization"))
                {
                    byte[] data = DataEntrySerialization.SyncSave(DataTestData.SyncTestEntry);
                    totalData += data.Length;

                    SyncTestEntry restoredSync = new SyncTestEntry();
                    DataEntrySerialization.SyncLoad(restoredSync, data);
                }
            }

            //var container = CarbonContainerAutofacBuilder.Build<CrystalBuildModule>();
            //container.Resolve<IMain>().Build();

            //Profiler.TraceProfilerStatistics();
        }
    }
}