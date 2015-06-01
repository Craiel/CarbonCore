namespace CarbonCore.Tests.Contracts
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public interface IFrameworkTestMainViewModel : IBaseViewModel
    {
        string Title { get; }
    }
}
