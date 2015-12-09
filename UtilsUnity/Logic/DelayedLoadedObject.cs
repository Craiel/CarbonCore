namespace CarbonCore.Utils.Unity.Logic
{
    using System;

    using CarbonCore.Utils.Unity.Contracts;
    using CarbonCore.Utils.Unity.Logic.Enums;

    public class DelayedLoadedObject : IDelayedLoadedObject
    {
        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public DelayedLoadedObject()
        {
            this.LoadPhase = DelayedLoadedObjectPhase.PreLoad;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event Action<DelayedLoadedObject> OnLoadStarted;
        public event Action<DelayedLoadedObject> OnLoadFinished;

        public bool IsLoading { get; private set; }

        public bool IsLoaded { get; private set; }

        public DelayedLoadedObjectPhase LoadPhase { get; private set; }

        public bool ContinueLoad()
        {
            if (!this.IsLoading)
            {
                this.IsLoading = true;
                this.NotifyLoadStarted();
                this.LoadPhase = DelayedLoadedObjectPhase.PreLoad;
            }

            if (this.DoContinueLoad(this.LoadPhase) || this.AdvanceLoadPhase())
            {
                // Still loading
                return true;
            }

            this.IsLoaded = true;

            this.NotifyLoadFinished();
            
            return false;
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual bool DoContinueLoad(DelayedLoadedObjectPhase phase)
        {
            return false;
        }

        protected void NotifyLoadStarted()
        {
            if (this.OnLoadStarted != null)
            {
                this.OnLoadStarted(this);
            }
        }

        protected void NotifyLoadFinished()
        {
            if (this.OnLoadFinished != null)
            {
                this.OnLoadFinished(this);
            }
        }

        private bool AdvanceLoadPhase()
        {
            switch (this.LoadPhase)
            {
                case DelayedLoadedObjectPhase.PreLoad:
                    {
                        this.LoadPhase = DelayedLoadedObjectPhase.Load;
                        return true;
                    }

                case DelayedLoadedObjectPhase.Load:
                    {
                        this.LoadPhase = DelayedLoadedObjectPhase.PostLoad;
                        return true;
                    }
            }

            return false;
        }
    }
}
