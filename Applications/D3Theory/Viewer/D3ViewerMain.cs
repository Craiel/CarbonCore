namespace CarbonCore.Applications.D3Theory.Viewer
{
    using System.Windows;

    using CarbonCore.Applications.D3Theory.Viewer.Contracts;
    using CarbonCore.Applications.D3Theory.Viewer.View;
    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.ToolFramework.Logic;
    using CarbonCore.Utils.Contracts.IoC;

    public class D3ViewerMain : WindowApplicationBase, ID3ViewerMain
    {
        private const string ToolName = "D3TheoryViewer";

        private readonly IFactory factory;
        
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public D3ViewerMain(IFactory factory)
            : base(factory)
        {
            this.factory = factory;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public override string Name
        {
            get
            {
                return ToolName;
            }
        }
        
        protected override IBaseViewModel DoInitializeMainViewModel()
        {
            return this.factory.Resolve<ID3ViewerMainViewModel>();
        }

        protected override Window DoInitializeMainWindow()
        {
            return new MainWindow();
        }
    }
}
