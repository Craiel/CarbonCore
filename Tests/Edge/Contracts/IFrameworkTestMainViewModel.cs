namespace CarbonCore.Tests.Edge.Contracts
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public interface IFrameworkTestMainViewModel : IBaseViewModel
    {
        string Title { get; }
    }
}
