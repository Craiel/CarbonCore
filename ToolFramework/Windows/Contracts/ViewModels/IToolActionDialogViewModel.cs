namespace CarbonCore.ToolFramework.Windows.Contracts.ViewModels
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Windows.Input;
    using System.Windows.Media;

    using CarbonCore.ToolFramework.Contracts;

    public enum ToolActionDisplayMode
    {
        Progress,
        Splash
    }

    public interface IToolActionDialogViewModel : IBaseViewModel
    {
        ImageSource Icon { get; set; }
        ImageSource Image { get; set; }

        ToolActionDisplayMode DisplayMode { get; set; }

        ReadOnlyObservableCollection<IToolActionViewModel> Actions { get; }

        bool CanCancel { get; }

        int MainProgress { get; }
        int MainProgressMax { get; }

        string MainProgressText { get; set; }
        
        ICommand CommandCancel { get; }

        void SetActions(IList<IToolAction> actions);
    }
}
