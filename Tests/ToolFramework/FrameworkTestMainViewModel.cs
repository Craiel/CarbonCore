namespace CarbonCore.Tests.ToolFramework
{
    using CarbonCore.Tests.Contracts;
    using CarbonCore.ToolFramework.ViewModel;
    using CarbonCore.Utils.Contracts.IoC;

    public class FrameworkTestMainViewModel : BaseViewModel, IFrameworkTestMainViewModel
    {
        private readonly IFrameworkTestMain main;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public FrameworkTestMainViewModel(IFactory factory)
        {
            this.main = factory.Resolve<IFrameworkTestMain>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Title
        {
            get
            {
                return this.main.Title;
            }
        }
    }
}
