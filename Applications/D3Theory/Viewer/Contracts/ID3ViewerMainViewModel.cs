namespace CarbonCore.Applications.D3Theory.Viewer.Contracts
{
    using CarbonCore.ToolFramework.Windows.Contracts.ViewModels;

    public interface ID3ViewerMainViewModel : IBaseViewModel
    {
        string Title { get; }
    }
}
