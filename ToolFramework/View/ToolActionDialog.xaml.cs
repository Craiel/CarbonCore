namespace CarbonCore.ToolFramework.View
{
    using System.Collections.Generic;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Contracts.ViewModels;
    using CarbonCore.Utils.Compat.Contracts.IoC;

    public partial class ToolActionDialog
    {
        public ToolActionDialog()
        {
            this.InitializeComponent();
        }

        // Static helper method to create an action dialog
        public static ToolActionDialog CreateNew(IFactory factory, IList<IToolAction> actions, ToolActionDisplayMode mode = ToolActionDisplayMode.Progress)
        {
            var vm = factory.Resolve<IToolActionDialogViewModel>();
            vm.SetActions(actions);
            vm.DisplayMode = mode;
            return new ToolActionDialog { DataContext = vm };
        }
    }
}
