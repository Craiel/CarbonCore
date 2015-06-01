namespace CarbonCore.ToolFramework.ViewModel
{
    using System;

    using CarbonCore.ToolFramework.Contracts;
    using CarbonCore.ToolFramework.Contracts.ViewModels;

    public class ToolActionViewModel : BaseViewModel, IToolActionViewModel
    {
        private IToolAction action;

        private bool isActive;

        private int progress;

        private int progressMax;

        private string progressText;
        
        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsActive
        {
            get
            {
                return this.isActive;
            }

            set
            {
                this.PropertySet(this.isActive, value, out this.isActive);
            }
        }

        public int Progress
        {
            get
            {
                return this.progress;
            }

            private set
            {
                this.PropertySet(this.progress, value, out this.progress);
            }
        }

        public int ProgressMax
        {
            get
            {
                return this.progressMax;
            }

            private set
            {
                this.PropertySet(this.progressMax, value, out this.progressMax);
            }
        }

        public string ProgressText
        {
            get
            {
                return this.progressText;
            }

            private set
            {
                this.PropertySet(ref this.progressText, value);
            }
        }

        public IToolActionResult Result { get; private set; }

        public void SetAction(IToolAction newAction)
        {
            if (this.action != null)
            {
                throw new InvalidOperationException("Action is already set");
            }

            this.action = newAction;
        }
        
        public void RefreshStatus()
        {
            this.IsActive = this.action.IsRunning;
            this.Progress = this.action.Progress;
            this.ProgressMax = this.action.ProgressMax;
            this.ProgressText = this.action.ProgressMessage;
            this.Result = this.action.Result;
        }
    }
}
