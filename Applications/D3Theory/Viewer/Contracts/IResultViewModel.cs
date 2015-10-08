namespace D3Theory.Viewer.Contracts
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;

    using CarbonCore.ToolFramework.Contracts.ViewModels;

    using D3Theory.Data;
    using D3Theory.Viewer.Logic;

    public interface IResultViewModel : IBaseViewModel
    {
        Image ClassIcon { get; }

        string ClassName { get; }

        DateTime Duration { get; }

        int KillCount { get; }
        CompareState KillCountCompareState { get; }
        
        ReadOnlyObservableCollection<IEntityAttributeViewModel> EntityStats { get; }
        ReadOnlyObservableCollection<ISimulationStatViewModel> SimulationStats { get; }
        ReadOnlyObservableCollection<IAttributeViewModel> MergedAttributes { get; }

        SampleViewMode SampleViewMode { get; set; }
        ReadOnlyObservableCollection<ISampleViewModel> Samples { get; }

        void LoadData(SimulationStats data);

        void CompareTo(IResultViewModel other);
    }
}
