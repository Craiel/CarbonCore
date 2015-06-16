namespace CarbonCore.ToolFramework.ViewModel
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Input;
    using System.Windows.Media;

    using CarbonCore.Resources;
    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.Utils.Compat.Contracts.IoC;
    using CarbonCore.UtilsWPF;
    using CarbonCore.UtilsWPF.Collections;

    public class ToolActionDialogViewModel : BaseViewModel, IToolActionDialogViewModel, IDisposable
    {
        private readonly IFactory factory;
        private readonly ExtendedObservableCollection<IToolActionViewModel> activeActions;

        private readonly CancellationTokenSource cancellationTokenSource;

        private ImageSource icon;
        private ImageSource image;

        private ToolActionDisplayMode displayMode;

        private IList<IToolAction> actions;

        private bool canCancel;

        private int mainProgress;
        private int mainProgressMax;

        private string mainProgressText;

        private bool isRunning;

        private ICommand commandCancel;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public ToolActionDialogViewModel(IFactory factory)
        {
            this.factory = factory;
            this.activeActions = new ExtendedObservableCollection<IToolActionViewModel>();
            this.cancellationTokenSource = new CancellationTokenSource();

            // Set some defaults for our visuals
            this.icon = Static.IconPlaceholderUri.ToImageSource();
            this.mainProgressText = "Please wait...";
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool CanCancel
        {
            get
            {
                return this.canCancel;
            }

            set
            {
                this.PropertySet(this.canCancel, value, out this.canCancel);
            }
        }

        public ImageSource Icon
        {
            get
            {
                return this.icon;
            }

            set
            {
                this.PropertySet(ref this.icon, value);
            }
        }

        public ImageSource Image
        {
            get
            {
                return this.image;
            }

            set
            {
                this.PropertySet(ref this.image, value);
            }
        }

        public ToolActionDisplayMode DisplayMode
        {
            get
            {
                if (!this.isRunning)
                {
                    this.isRunning = true;
                    Task.Factory.StartNew(this.BeginActions);
                }

                return this.displayMode;
            }

            set
            {
                this.PropertySet(this.displayMode, value, out this.displayMode);
            }
        }

        public ReadOnlyObservableCollection<IToolActionViewModel> Actions
        {
            get
            {
                return this.activeActions.AsReadOnly;
            }
        }

        public int MainProgress
        {
            get
            {
                return this.mainProgress;
            }

            private set
            {
                this.PropertySet(this.mainProgress, value, out this.mainProgress);
            }
        }

        public int MainProgressMax
        {
            get
            {
                return this.mainProgressMax;
            }

            private set
            {
                this.PropertySet(this.mainProgressMax, value, out this.mainProgressMax);
            }
        }

        public string MainProgressText
        {
            get
            {
                return this.mainProgressText;
            }

            set
            {
                this.PropertySet(ref this.mainProgressText, value);
            }
        }

        public ICommand CommandCancel
        {
            get
            {
                return this.commandCancel ?? (this.commandCancel = new RelayCommand(this.OnCancel, () => this.CanCancel));
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetActions(IList<IToolAction> newActions)
        {
            if (this.actions != null)
            {
                throw new InvalidOperationException("Actions are already set");
            }

            this.actions = newActions;

            this.MainProgress = 0;
            this.MainProgressMax = newActions.Count;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.cancellationTokenSource.Dispose();
            }
        }

        // -------------------------------------------------------------------
        // Private
        // -------------------------------------------------------------------
        private void OnCancel()
        {
            this.cancellationTokenSource.Cancel();
        }

        private void BeginActions()
        {
            this.CanCancel = false;
            this.MainProgressMax = this.actions.Count;
            IDictionary<int, IList<IToolAction>> orderDictionary = new Dictionary<int, IList<IToolAction>>();
            foreach (IToolAction action in this.actions)
            {
                if (action.CanCancel)
                {
                    this.CanCancel = true;
                }

                if (!orderDictionary.ContainsKey(action.Order))
                {
                    orderDictionary.Add(action.Order, new List<IToolAction>());
                }

                orderDictionary[action.Order].Add(action);
            }

            // Execute the view models by order
            foreach (int i in orderDictionary.Keys.OrderBy(x => x))
            {
                this.activeActions.Clear();

                foreach (IToolAction action in orderDictionary[i])
                {
                    if (this.cancellationTokenSource.IsCancellationRequested)
                    {
                        break;
                    }

                    var vm = this.factory.Resolve<IToolActionViewModel>();
                    vm.SetAction(action);
                    this.activeActions.Add(vm);
                }

                // Process events to update the new view models
                this.DoEvents();

                // Execute the actions
                foreach (IToolAction action in orderDictionary[i])
                {
                    action.Execute(this.cancellationTokenSource.Token);
                }

                // Refresh the view model state
                foreach (IToolActionViewModel viewModel in this.activeActions)
                {
                    viewModel.RefreshStatus();
                }

                // Do another event to update the started state
                this.DoEvents();

                while (this.activeActions.Any(x => x.IsActive) && !this.cancellationTokenSource.IsCancellationRequested)
                {
                    foreach (IToolActionViewModel viewModel in this.activeActions)
                    {
                        viewModel.RefreshStatus();
                    }

                    Thread.Sleep(100);
                    this.DoEvents();
                }

                this.MainProgress += this.activeActions.Count;
            }
        }
    }
}
