namespace D3Theory.Viewer.Contracts
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    using D3Theory.Data;
    using D3Theory.Viewer.Logic;

    public interface IEntityAttributeViewModel : IBaseViewModel
    {
        D3EntityAttribute Attribute { get; }

        float Value { get; }

        string DisplayValue { get; }

        CompareState CompareState { get; }

        void CompareTo(IEntityAttributeViewModel other);
    }
}
