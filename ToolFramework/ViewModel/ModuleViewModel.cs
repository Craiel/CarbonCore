namespace CarbonCore.ToolFramework.ViewModel
{
    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.Utils.Compat.Contracts.IoC;

    public abstract class ModuleViewModel : BaseViewModel, IModuleViewModel
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        protected ModuleViewModel(IFactory factory)
        {
            this.LogViewModel = factory.Resolve<ILogViewModel>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public abstract string Title { get; }

        public ILogViewModel LogViewModel { get; private set; }
    }
}
