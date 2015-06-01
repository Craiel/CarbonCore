namespace CarbonCore.ToolFramework.Contracts.ViewModels
{
    using System;
    using System.Collections.ObjectModel;
    using System.Windows.Input;

    public interface ILogViewModel : IBaseViewModel, IDisposable
    {
        int EntryCount { get; }

        int ErrorCount { get; }
        int WarningCount { get; }
        int InfoCount { get; }

        ReadOnlyObservableCollection<ILogEntryViewModel> FilteredEntries { get; }

        ICommand CommandToggleError { get; }
        ICommand CommandToggleWarning { get; }
        ICommand CommandToggleInfo { get; }
    }
}
