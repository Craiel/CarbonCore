namespace CarbonCore.CrystalBuild.Contracts
{
    using CarbonCore.Utils.IO;

    public interface IContentProcessor
    {
        void Process(CarbonFile file);

        string GetData();

        void SetContext<T>(T newContext) where T : class, IProcessingContext;

        T GetContext<T>() where T : class, IProcessingContext;
    }
}
