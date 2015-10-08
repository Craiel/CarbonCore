namespace D3Theory.Viewer.Contracts
{
    using System.Collections.ObjectModel;
    using System.Windows.Controls;

    using CarbonCore.ToolFramework.Contracts.ViewModels;

    using D3Theory.Data;
    using D3Theory.Viewer.Logic;

    public interface ISampleViewModel : IBaseViewModel
    {
        Image Icon { get; }

        string Name { get; }

        D3DamageType? DamageType { get; }

        ReadOnlyObservableCollection<ISimulationStatViewModel> SampleStats { get; }

        float DamageTotal { get; }
        CompareState DamageTotalCompareState { get; }

        float DamageMin { get; }
        CompareState DamageMinCompareState { get; }

        float DamageMax { get; }
        CompareState DamageMaxCompareState { get; }

        float DamageAverage { get; }
        CompareState DamageAverageCompareState { get; }

        float DamageCount { get; }
        CompareState DamageCountCompareState { get; }

        float DamageNormal { get; }
        CompareState DamageNormalCompareState { get; }

        float DamageCrit { get; }
        CompareState DamageCritCompareState { get; }

        void CompareTo(ISampleViewModel other);
    }
}
