namespace CarbonCore.ToolFramework.Contracts
{
    using System.ComponentModel;
    using System.Windows.Input;

    public delegate bool PropertyChangingCancellableEventHandler(object sender, PropertyChangingEventArgs args);

    public delegate void PropertyChangedDetailedEventHandler(object sender, object oldValue, object newValue, PropertyChangedEventArgs args);

    public interface IBaseViewModel : INotifyPropertyChanged
    {
        event PropertyChangingCancellableEventHandler PropertyChanging;
        event PropertyChangedDetailedEventHandler PropertyChangedDetailed;

        ICommand CommandUndo { get; }
        ICommand CommandRedo { get; }
    }
}
