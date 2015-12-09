namespace CarbonCore.Utils.Unity.Logic
{
    using System;
    using System.Collections.Generic;

    using CarbonCore.Utils.Diagnostics;
    using CarbonCore.Utils.Threading;
    using CarbonCore.Utils.Unity.Contracts;

    public abstract class EngineComponent : IEngineComponent
    {
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
            Diagnostic.Assert(this.IsInitialized, "Update called before Initialize for " + this.GetType().Name);

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
                Diagnostic.Warning("Tried to remove non-existing Child Component");
                return;
            }

            this.childComponents.Remove(component);
        }
    }
}
