namespace CarbonCore.Utils.IoC
{
    using System;
    using System.Collections.Generic;

    using Autofac;
    using Autofac.Core;
    using Autofac.Core.Lifetime;
    using Autofac.Core.Resolving;

    using CarbonCore.Utils.Compat.Contracts.IoC;

    public class CarbonContainerAutofac : ICarbonContainer, IContainer
    {
        private readonly IContainer innerContainer;

        // -------------------------------------------------------------------
        // Constructor
        // -------------------------------------------------------------------
        public CarbonContainerAutofac(IContainer innerContainer)
        {
            this.innerContainer = innerContainer;
            this.innerContainer.ChildLifetimeScopeBeginning += this.ChildLifetimeScopeBeginning;
            this.innerContainer.CurrentScopeEnding += this.CurrentScopeEnding;
            this.innerContainer.ResolveOperationBeginning += this.ResolveOperationBeginning;
        }

        // -------------------------------------------------------------------
        // Public
        // -------------------------------------------------------------------
        public event EventHandler<LifetimeScopeBeginningEventArgs> ChildLifetimeScopeBeginning;
        public event EventHandler<LifetimeScopeEndingEventArgs> CurrentScopeEnding;
        public event EventHandler<ResolveOperationBeginningEventArgs> ResolveOperationBeginning;

        public IComponentRegistry ComponentRegistry
        {
            get
            {
                return this.innerContainer.ComponentRegistry;
            }
        }
        
        public IDisposer Disposer
        {
            get
            {
                return this.innerContainer.Disposer;
            }
        }

        public object Tag
        {
            get
            {
                return this.innerContainer.Tag;
            }
        }

        public object ResolveComponent(IComponentRegistration registration, IEnumerable<Parameter> parameters)
        {
            return this.innerContainer.ResolveComponent(registration, parameters);
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ILifetimeScope BeginLifetimeScope()
        {
            return this.innerContainer.BeginLifetimeScope();
        }

        public ILifetimeScope BeginLifetimeScope(object tag)
        {
            return this.innerContainer.BeginLifetimeScope(tag);
        }

        public ILifetimeScope BeginLifetimeScope(Action<ContainerBuilder> configurationAction)
        {
            return this.innerContainer.BeginLifetimeScope(configurationAction);
        }

        public ILifetimeScope BeginLifetimeScope(object tag, Action<ContainerBuilder> configurationAction)
        {
            return this.innerContainer.BeginLifetimeScope(tag, configurationAction);
        }

        public T Resolve<T>()
        {
            return this.innerContainer.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return this.innerContainer.Resolve(type);
        }

        public T Resolve<T>(params Parameter[] parameter)
        {
            return this.innerContainer.Resolve<T>(parameter);
        }

        // -------------------------------------------------------------------
        // Protected
        // -------------------------------------------------------------------
        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            this.innerContainer.ChildLifetimeScopeBeginning -= this.ChildLifetimeScopeBeginning;
            this.innerContainer.CurrentScopeEnding -= this.CurrentScopeEnding;
            this.innerContainer.ResolveOperationBeginning -= this.ResolveOperationBeginning;
            this.innerContainer.Dispose();
        }
    }
}
