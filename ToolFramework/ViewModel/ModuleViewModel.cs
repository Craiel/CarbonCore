namespace CarbonCore.ToolFramework.ViewModel
{
    using CarbonCore.ToolFramework.Contracts;

    public abstract class ModuleViewModel : BaseViewModel, IModuleViewModel
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Title { get; }
    }
}
