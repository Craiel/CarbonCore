namespace CarbonCore.Tests.Edge.ToolFramework
{
    using CarbonCore.Tests.Edge.Contracts;
    using CarbonCore.ToolFramework.Windows.ViewModel;

    public class FrameworkTestMainViewModel : BaseViewModel, IFrameworkTestMainViewModel
    {
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public string Title
        {
            get
            {
                return "Framework Test";
            }
        }
    }
}
