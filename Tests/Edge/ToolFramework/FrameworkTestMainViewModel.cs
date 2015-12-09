namespace CarbonCore.Tests.ToolFramework
{
    using CarbonCore.Tests.Contracts;
    using CarbonCore.ToolFramework.ViewModel;

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
