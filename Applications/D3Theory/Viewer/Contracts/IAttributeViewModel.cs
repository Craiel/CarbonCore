namespace D3Theory.Viewer.Contracts
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    using D3Theory.Data;
    using D3Theory.Viewer.Logic;

    public interface IAttributeViewModel : IBaseViewModel
    {
        D3Attribute Attribute { get; }

        float Value { get; }

        string DisplayValue { get; }

        CompareState CompareState { get; }

        void CompareTo(IAttributeViewModel other);
    }
}
