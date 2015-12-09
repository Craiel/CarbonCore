namespace CarbonCore.Applications.D3Theory.Viewer.Contracts
{
    using CarbonCore.Applications.D3Theory.Viewer.Logic;
    using CarbonCore.Modules.D3Theory.Data;
    using CarbonCore.ToolFramework.Windows.Contracts.ViewModels;

    public interface IAttributeViewModel : IBaseViewModel
    {
        D3Attribute Attribute { get; }

        float Value { get; }

        string DisplayValue { get; }

        CompareState CompareState { get; }

        void CompareTo(IAttributeViewModel other);
    }
}
