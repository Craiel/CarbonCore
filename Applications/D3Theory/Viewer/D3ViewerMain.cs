namespace D3Theory.Viewer
{
    using System.Windows;

    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.ToolFramework.Logic;
    using CarbonCore.Utils.Compat.Contracts.IoC;

    using D3Theory.Viewer.Contracts;
    using D3Theory.Viewer.View;

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
