namespace CarbonCore.ToolFramework.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Threading;
    using System.Windows.Input;

    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.Utils.Compat.Diagnostics;
    using CarbonCore.Utils.Diagnostics.TraceListeners;
    using CarbonCore.UtilsWPF;
    using CarbonCore.UtilsWPF.Collections;

    public class LogViewModel : BaseViewModel, ILogViewModel
    {
        private readonly IFactory factory;

        private readonly IList<ILogEntryViewModel> entries;

        private readonly ExtendedObservableCollection<ILogEntryViewModel> filteredEntries;

        private readonly IDictionary<TraceEventType, bool> eventEnableStatus;

        private readonly IDictionary<TraceEventType, int> eventCount;

        private readonly Timer updateTimer;

        private ICommand commandToggleError;
        private ICommand commandToggleWarning;
        private ICommand commandToggleInfo;

        public LogViewModel(IFactory factory)
        {
            this.factory = factory;

            this.entries = new List<ILogEntryViewModel>();
            this.filteredEntries = new ExtendedObservableCollection<ILogEntryViewModel>();
            this.eventEnableStatus = new Dictionary<TraceEventType, bool>();
            this.eventCount = new Dictionary<TraceEventType, int>();

            // Enable all events by default
            foreach (TraceEventType type in Enum.GetValues(typeof(TraceEventType)))
            {
                this.eventEnableStatus.Add(type, true);
                this.eventCount.Add(type, 0);
            }

            this.updateTimer = new Timer(this.OnPollEvents, null, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(100));
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public int EntryCount
        {
            get
            {
                return this.entries.Count;
            }
        }

        public int ErrorCount
        {
            get
            {
                return this.eventCount[TraceEventType.Error];
            }
        }

        public int WarningCount
        {
            get
            {
                return this.eventCount[TraceEventType.Warning];
            }
        }

        public int InfoCount
        {
            get
            {
                return this.eventCount[TraceEventType.Information];
            }
        }

        public ReadOnlyObservableCollection<ILogEntryViewModel> FilteredEntries
        {
            get
            {
                return this.filteredEntries.AsReadOnly;
            }
        }

        public ICommand CommandToggleError
        {
            get
            {
                return this.commandToggleError ?? (this.commandToggleError = new RelayCommand(() => this.OnToggle(TraceEventType.Error)));
            }
        }

        public ICommand CommandToggleWarning
        {
            get
            {
                return this.commandToggleWarning ?? (this.commandToggleWarning = new RelayCommand(() => this.OnToggle(TraceEventType.Warning)));
            }
        }

        public ICommand CommandToggleInfo
        {
            get
            {
                return this.commandToggleInfo ?? (this.commandToggleInfo = new RelayCommand(() => this.OnToggle(TraceEventType.Information)));
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void Dispose(bool isDisposing)
        {
            if (isDisposing)
            {
                this.updateTimer.Dispose();
            }
        }

        private void OnToggle(TraceEventType type)
        {
            this.eventEnableStatus[type] = !this.eventEnableStatus[type];
            this.UpdateFilteredEntries();
        }

        private void UpdateFilteredEntries()
        {
            this.filteredEntries.Clear();
            using (this.filteredEntries.BeginSuspendNotification())
            {
                foreach (ILogEntryViewModel entry in this.entries)
                {
                    if (!this.eventEnableStatus[entry.Type])
                    {
                        continue;
                    }

                    this.filteredEntries.Add(entry);
                }
            }
        }

        private void ReceiveEvent(TraceEventData data)
        {
            var vm = this.factory.Resolve<ILogEntryViewModel>();
            vm.SetData(data);
            this.entries.Add(vm);

            if (this.eventEnableStatus[data.Type])
            {
                this.DispatchIfNeeded(() => this.filteredEntries.Add(vm));
            }

            this.eventCount[data.Type]++;
            this.NotifyPropertyChangedAll();
        }

        private void OnPollEvents(object state)
        {
            IList<TraceEventData> data = EventTraceListener.PollEventData();
            foreach (TraceEventData eventData in data)
            {
                this.ReceiveEvent(eventData);
            }
        }
    }
}
