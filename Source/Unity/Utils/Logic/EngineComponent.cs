namespace CarbonCore.Unity.Utils.Logic
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;

    using CarbonCore.Unity.Utils.Contracts;
    using CarbonCore.Utils.Threading;

    using NLog;

    public abstract class EngineComponent : IEngineComponent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private IList<IEngineComponent> childComponents;

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public bool IsInitialized { get; private set; }

        public virtual void Initialize()
        {
            if (this.childComponents != null)
            {
                foreach (IEngineComponent component in this.childComponents)
                {
                    component.Initialize();
                }
            }

            this.IsInitialized = true;
        }

        public virtual void Destroy()
        {
            if (this.childComponents != null)
            {
                foreach (IEngineComponent component in this.childComponents)
                {
                    component.Destroy();
                }
            }

            this.IsInitialized = false;

            // Try to dispose if we implement it
            var disposable = this as IDisposable;
            if (disposable != null)
            {
                disposable.Dispose();
            }
        }

        public virtual void Update(EngineTime time)
        {
            Debug.Assert(this.IsInitialized, "Update called before Initialize for " + this.GetType().Name);

            if (this.childComponents != null)
            {
                foreach (IEngineComponent component in this.childComponents)
                {
                    component.Update(time);
                }
            }
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected void AddChildComponent(IEngineComponent component)
        {
            if (this.childComponents == null)
            {
                this.childComponents = new List<IEngineComponent>();
            }

            this.childComponents.Add(component);
        }

        protected void RemoveChildComponent(IEngineComponent component)
        {
            if (this.childComponents == null || !this.childComponents.Contains(component))
            {
                Logger.Warn("Tried to remove non-existing Child Component");
                return;
            }

            this.childComponents.Remove(component);
        }
    }
}
