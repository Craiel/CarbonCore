namespace CarbonCore.Applications.D3Theory.Viewer.Contracts
{
    using CarbonCore.Applications.D3Theory.Viewer.Logic;
    using CarbonCore.Modules.D3Theory.Data;
    using CarbonCore.ToolFramework.Windows.Contracts.ViewModels;

    public interface IEntityAttributeViewModel : IBaseViewModel
    {
        D3EntityAttribute Attribute { get; }

        float Value { get; }

        string DisplayValue { get; }

        CompareState CompareState { get; }

        void CompareTo(IEntityAttributeViewModel other);
    }
}
