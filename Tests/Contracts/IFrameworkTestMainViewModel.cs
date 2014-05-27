namespace CarbonCore.Tests.Contracts
{
    using CarbonCore.ToolFramework.Contracts;

    public interface IFrameworkTestMainViewModel : IBaseViewModel
    {
        string Title { get; }
    }
}
