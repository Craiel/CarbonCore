﻿namespace D3Theory.Viewer.ViewModel
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Controls;

    using CarbonCore.ToolFramework.ViewModel;
    using CarbonCore.Utils.Compat.Contracts.IoC;

    using D3Theory.Data;
    using D3Theory.Viewer.Contracts;
    using D3Theory.Viewer.Logic;

    public class ResultViewModel : BaseViewModel, IResultViewModel
    {
        private SimulationStats data;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ResultViewModel(IFactory factory)
        {
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public Image ClassIcon { get; private set; }

        public string ClassName { get; private set; }

        public DateTime Duration { get; private set; }

        public int KillCount { get; private set; }

        public CompareState KillCountCompareState { get; private set; }

        public ReadOnlyObservableCollection<IEntityAttributeViewModel> EntityStats { get; private set; }

        public ReadOnlyObservableCollection<ISimulationStatViewModel> SimulationStats { get; private set; }

        public ReadOnlyObservableCollection<IAttributeViewModel> MergedAttributes { get; private set; }

        public SampleViewMode SampleViewMode { get; set; }

        public ReadOnlyObservableCollection<ISampleViewModel> Samples { get; private set; }

        public void LoadData(SimulationStats newData)
        {
            this.data = newData;
            this.UpdateViewModel();
        }

        public void CompareTo(IResultViewModel other)
        {
            throw new NotImplementedException();
        }

        private void UpdateViewModel()
        {
            throw new NotImplementedException();
        }
    }
}