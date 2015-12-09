namespace CarbonCore.Applications.D3Theory.Viewer.ViewModel
{
    using CarbonCore.Applications.D3Theory.Viewer.Contracts;
    using CarbonCore.ToolFramework.Windows.ViewModel;
    using CarbonCore.Utils.Contracts.IoC;

    public class D3ViewerMainViewModel : BaseViewModel, ID3ViewerMainViewModel
    {
        private readonly ID3ViewerMain main;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public D3ViewerMainViewModel(IFactory factory)
        {
            this.main = factory.Resolve<ID3ViewerMain>();
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Title
        {
            get
            {
                return this.main.Name;
            }
        }
    }
}
