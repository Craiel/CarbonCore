namespace CarbonCore.Applications.CrystalBuild.Contracts.Processors
{
    using CarbonCore.Utils.IO;

    public interface IContentProcessor
    {
        void Process(CarbonFile file);

        string GetData();
    }
}
