namespace CarbonCore.Applications.CrystalBuild.Contracts.Processors
{
    using CarbonCore.Applications.CrystalBuild.Logic;
    using CarbonCore.Utils.IO;

    public interface IContentProcessor
    {
        ProcessingContext Context { get; }
        
        void Process(CarbonFile file);

        string GetData();

        void SetContext(ProcessingContext context);
    }
}
