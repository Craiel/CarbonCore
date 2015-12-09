namespace CarbonCore.Tests.Edge.Contracts
{
    using CarbonCore.ToolFramework.Windows.Contracts.ViewModels;

    public interface IFrameworkTestMainViewModel : IBaseViewModel
    {
        string Title { get; }
    }
}
