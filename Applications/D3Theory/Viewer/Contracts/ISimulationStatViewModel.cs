namespace D3Theory.Viewer.Contracts
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    using D3Theory.Data;
    using D3Theory.Viewer.Logic;

    public interface ISimulationStatViewModel : IBaseViewModel
    {
        SimulationStat Stat { get; }

        float Value { get; }

        string DisplayValue { get; }

        CompareState CompareState { get; }

        void CompareTo(ISimulationStatViewModel other);
    }
}
