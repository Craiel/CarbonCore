namespace CarbonCore.Applications.D3Theory.Viewer.Contracts
{
    using CarbonCore.Applications.D3Theory.Viewer.Logic;
    using CarbonCore.Modules.D3Theory.Data;
    using CarbonCore.ToolFramework.Windows.Contracts.ViewModels;

    public interface ISimulationStatViewModel : IBaseViewModel
    {
        SimulationStat Stat { get; }

        float Value { get; }

        string DisplayValue { get; }

        CompareState CompareState { get; }

        void CompareTo(ISimulationStatViewModel other);
    }
}
