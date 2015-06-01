namespace CarbonCore.ToolFramework.Contracts.ViewModels
{
    public interface IToolActionViewModel : IBaseViewModel
    {
        bool IsActive { get; }

        int Progress { get; }
        int ProgressMax { get; }

        string ProgressText { get; }

        IToolActionResult Result { get; }

        void SetAction(IToolAction action);
        
        void RefreshStatus();
    }
}
