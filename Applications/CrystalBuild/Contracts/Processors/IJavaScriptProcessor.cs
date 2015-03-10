namespace CarbonCore.Applications.CrystalBuild.Contracts.Processors
{
    public interface IJavaScriptProcessor : IContentProcessor
    {
        bool IsDebug { get; set; }
    }
}
