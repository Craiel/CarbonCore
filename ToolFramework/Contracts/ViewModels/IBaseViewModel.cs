namespace CarbonCore.ToolFramework.Contracts.ViewModels
{
    using System.ComponentModel;

    public delegate bool PropertyChangingCancellableEventHandler(object sender, PropertyChangingEventArgs args);

    public delegate void PropertyChangedDetailedEventHandler(object sender, object oldValue, object newValue, PropertyChangedEventArgs args);

    public interface IBaseViewModel : INotifyPropertyChanged
    {
        event PropertyChangingCancellableEventHandler PropertyChanging;
        event PropertyChangedDetailedEventHandler PropertyChangedDetailed;

        void Initialize();

        void Invalidate();
    }
}
